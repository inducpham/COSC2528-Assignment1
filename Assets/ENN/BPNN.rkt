#lang r6rs
(import (rnrs lists (6))
        (rnrs base (6))
        (rnrs io simple (6)))

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;some unecessary things

(define-syntax with-map-operation
  (syntax-rules (quote define let let* unmap on)
    [(_ (_ on procedures ...) (quote rest ...)) (quote rest ...)]
    [(_ (map-function on procedures ...) (define a body ...)) 
     (define a (with-map-operation (map-function on procedures ...) body) ...)]
    [(_ (map-function on procedures ...) (let [(v a)...] body ... ))
     (let [(v (with-map-operation (map-function on procedures ...) a))...]
       (with-map-operation (map-function on procedures ...) body)...)]
    [(_ (map-function on procedures ...) (let* [(v a)...] body ... ))
     (let* [(v (with-map-operation (map-function on procedures ...) a))...]
       (with-map-operation (map-function on procedures ...) body)...)]
    ;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
    [(_ (map-function on procedures ...) (unmap rest ...) other ...) 
     (begin (begin rest ...) (with-map-operation (map-function on procedures ...) other) ...)]
    [(_ (map-function on procedures ...) (operation var ...) body ...)
     (let-syntax [(mapping-operation
                   (syntax-rules (procedures ...)
                     [(_ procedures) (map-function procedures ;mark
                                          (with-map-operation (map-function on procedures ...) var) ...)] ...
                     [(_ else) (else 
                                     (with-map-operation (map-function on procedures ...) var)...)]))]
       (begin (mapping-operation operation)
              (with-map-operation (map-function on procedures ...) body)... ))]
    [(_ (_ on procedures ...) var)
     var]
    [(_ (map-function on procedures ...) var body ...)
     (var (with-map-operation (map-function on procedures ...) body) ...)]
    
    [(_ (_ on procedures ...) ()) ()]))


(define-syntax ea ;use human readable syntax for elementary arithmetic 
 (syntax-rules (+ - * /)
  [(_ ((A ...) rest ...))
   (let ((tmp (ea (A ...))))
    (ea (tmp rest ...)))]
  [(_ (A + B ...))
   (+ A (ea (B ...)))]
  [(_ (A - B rest ...))
   (let ((tmp (- (ea (B)))))
    (+ A (ea (tmp rest ...))))]
  [(_ (A * B rest ...))
   (let ((tmp (* A (ea (B)))))
    (ea (tmp rest ...)))]
  [(_ (A / B rest ...))
   (let ((tmp (/ A (ea (B)))))
    (ea (tmp rest ...)))]
  [(_ (A)) A]))


(define (map-tree f . rest)
  (define (mpair? rest) 
    (if (pair? rest) 
        (and (pair? (car rest)) (mpair? (cdr rest)))
        #t))
  (define (mnull? rest) 
    (if (pair? rest) 
        (and (null? (car rest)) (mnull? (cdr rest)))
        #t))
  (if (pair? rest)
      (if (mpair?  rest)
          (cons (apply map-tree (cons f  (map car rest)))
                 ;(map-tree f . (mcdr  rest)))
                 (apply map-tree (cons f (map cdr rest))))
          (if (mnull? rest)
              '()
              (apply f  rest)))
      '()))

  


;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;NN start here
                                 
      
(define (sum x) (apply + x))

(define bias 1)

(define act-fun (lambda (x) (/ 1 (+ 1.0 (exp (- x))))));activate function

(define (create-list-by-len value-generator len )
  (if (= len 0) '()
      (cons (value-generator) (create-list-by-len value-generator (- len 1)))))

(define (sim-neuro neuro inputs)
  (act-fun (sum (map * neuro inputs))))

(define (sim-layer layer inputs)
  (map (lambda (n) (sim-neuro n inputs)) layer))

(define (sim-nn nn inputs)
  (if (pair? nn)
      (if (pair? (cdr nn))
          (sim-nn (cdr nn) (sim-layer (car nn) (cons bias inputs))) ;input to the rest of the layers
          (map (lambda (x) (sum (map * x (cons bias inputs)))) (car nn)));output layer, with linear activate function
      inputs))

(define zeros (lambda()0))

(define (new-nn weigh-generator structure)
  (if (pair? structure)
      (if (pair? (cdr structure))
          (let ((gen-neuro (lambda () (create-list-by-len  weigh-generator (+ 1 (car structure))))))
            (let ((gen-layer (lambda () (create-list-by-len gen-neuro (cadr structure) ))))
              (cons (gen-layer) (new-nn weigh-generator (cdr structure)))))
          '())
      '()))

(define nnn (new-nn (lambda()1) '(2 2)))

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;start bp here

(define step 1)

(define (delta_i+1->delta_i layer delta_i+1 outputs_i)
  (if (null? outputs_i) '()
      (cons (* (sum (map * (map car layer) delta_i+1)) (car outputs_i) (- 1 (car outputs_i)))
            (delta_i+1->delta_i (map cdr layer) delta_i+1 (cdr outputs_i)))))

(define (update-layer layer delta_i inputs)
  (map ;for each neuro
   (lambda (neuro delta_ik)
     (map (lambda (w input) ;for each wight
            (+ w (* step  delta_ik  input)));mark 
          neuro  inputs))
   layer delta_i))

(define (train-nn nn targets inputs)              
  (if (pair? nn)
      (let ((inputs (cons 1 inputs)))
        (if (pair? (cdr nn))
            ;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
            ;;inner layer
            (let*
                ([outputs_i (sim-layer (car nn) inputs)]
                 [tmp (train-nn (cdr nn) targets outputs_i)]
                 [delta_i+1 (car tmp)]
                 [new-nn (cdr tmp)]
                 [delta_i   (delta_i+1->delta_i (cadr nn) delta_i+1 outputs_i)]
                 [new-layer (update-layer (car nn) delta_i inputs)])
              (cons delta_i (cons new-layer new-nn)))
            ;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
            ;output layer
            (let* ([outputs (map (lambda (x) (sum (map * x inputs))) (car nn))]
                   [error (map - targets outputs)]
                   [new-layer (update-layer (car nn) error inputs)]) ;output layer, with linear activate function
              ;(display error)(newline)
              (cons error (cons new-layer '())))))
      inputs))


;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;;test code

(define (gn)
  (define a 0)
  (lambda ()
    (set! a (+ a .1))
    a))

(define nnnn (new-nn (lambda () 1) '(2 2 1)))

(define (test-bp)
  (define nnnn (new-nn (gn) '(2 5 1)))
  (let dotimes ((n 600))
    ;(display nnnn) (newline)
    (set! nnnn (cdr (train-nn nnnn '(0) '(0 0))))
    (set! nnnn (cdr (train-nn nnnn '(1) '(1 0))))
    (set! nnnn (cdr (train-nn nnnn '(1) '(0 1))))
    (set! nnnn (cdr (train-nn nnnn '(0) '(1 1))))
    (if (> n 0) 
        (dotimes (- n 1))
        nnnn)))

(define new-nnn (test-bp))
(display (sim-nn new-nnn '(0 0)))
(display (sim-nn new-nnn '(1 0)))
(display (sim-nn new-nnn '(0 1)))
(display (sim-nn new-nnn '(1 1)))


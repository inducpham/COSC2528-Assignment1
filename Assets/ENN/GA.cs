using UnityEngine;
using System.Collections;
using IronScheme;
public class GA : IronSchemeGlobalDefine {

	static int numOfElite=5;
	static float mutationRate=0.0f;//1%
	static float mutationMagnitude=0.0f;//99%~101%
	static GA(){
		IronScheme.RuntimeExtensions.Eval (@"
		(define (fixed-length-priority-queue compair len)
		  (define queue (make-vector len 0))
		  (define (gq i) (vector-ref queue i))
		  (define (cmp a b) (compair (gq a) (gq b)))
		  (define (swap a)
		    (define min a)
		    (define b (+ 1 (* 2 a)))
		    (define c (+ 2 (* 2 a)))
		    (when (< b len)
		      (set! min (if (<= (cmp min b) 0) min b))
		      (when (< c len)
		        (set! min (if (<= (cmp min c) 0) min c)))
		      (unless (= min a)
		        (let [(tmp (gq min))]
		          (vector-set! queue min (gq a))
		          (vector-set! queue a tmp))
		        (swap min))))
		  (lambda (operator . rest)
		    (if (equal? operator 'push)
		        (let loop [(rest rest)]
		          (when (and (not (null? rest))(> (compair (car rest) (gq 0)) 0))
		            (vector-set! queue 0 (car rest))
		            (swap 0)
		            (loop (cdr rest))))
		        (if (equal? operator 'get) queue))))

		(define (gen-mutate-operator rand-fun mutation-rate magnitude)
		  (lambda (a)
		    (if (< (rand-fun 0 1) mutation-rate)
		        (+ a (* a (rand-fun (- magnitude) magnitude)))
		        a)))

		(define (gen-cross-operation rand-fun n)
		  (let ([cross-point-a (rand-fun 0 n)]
		        [cross-point-b (rand-fun 0 n)]
		        [counter 0])
		    (lambda (pair)
		      (when (>= counter cross-point-a)
		        (set! pair `(,(cadr pair) ,(car pair))))
		      (when (>= counter cross-point-b)
		        (set! pair `(,(cadr pair) ,(car pair))))
		      (inc! counter)
		      pair)))

		(define (cross cross-operator mutation-operator parents)
		  (if (null? parents) '()
		      (if (null? (car parents)) parents
		          (map cons (if (pair? (caar parents))
		                        (cross cross-operator mutation-operator (map car parents))
		                        (cross-operator (map 
		                                    (lambda (a) 
		                                      (mutation-operator (car a)))
		                                    parents)))
		               (cross cross-operator mutation-operator (map cdr parents))))))
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;settings
		(define mute-oper (gen-mutate-operator randf {0} {1}))
		",(double)mutationRate,(double)mutationMagnitude);
	}
	public void NextGeneration(){
		Individual[] population= GetComponentsInChildren<Individual> ();
		float[] wheel = new float[population.Length];
		for (int i=0; i+1<wheel.Length; i++) {
			wheel[i+1]+=wheel[i];		
		}
	}
	object getParents(float[] wheel,Individual[] population){
		float ps1 = Random.Range (0, wheel [wheel.Length - 1]);
		float ps2 = Random.Range (0, wheel [wheel.Length - 1]);
		Individual p1 = population[population.Length-1];
		Individual p2 = population[population.Length-1];
		for (int i=0; i<wheel.Length; i++) {
			if(ps1<wheel[i])
				p1=population[i];
			if(ps2<wheel[i])
				p2=population[i];
		}
		return IronScheme.RuntimeExtensions.Eval ("(list {0} {1})", p1.Gene, p2.Gene);
	}
}

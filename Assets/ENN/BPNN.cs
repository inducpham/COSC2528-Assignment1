using UnityEngine;
using System.Collections;
using IronScheme;


/// <summary>
/// Example code for training an XOR operation
/// Might cause a little bit of time for looping 600 times IN U3D.
///
/*
	BPNN nn = new BPNN (2, 5, 1);
	for (int i=0; i<600; i++) {
		nn.Train (new object[]{0},new object[]{0,0});	
		nn.Train (new object[]{1},new object[]{1,0});	
		nn.Train (new object[]{1},new object[]{0,1});	
		nn.Train (new object[]{0},new object[]{1,1});	
	}
	Debug.Log (((float[])nn.SimNN (0, 0))[0]);
	Debug.Log (((float[])nn.SimNN (0, 1))[0]);
	Debug.Log (((float[])nn.SimNN (1, 0))[0]);
	Debug.Log (((float[])nn.SimNN (1, 1))[0]);
*/
/// </summary>
public class BPNN : NN {
	/// <summary>
	/// positive number to determin the converge speed and stability, 
	/// better use simulated annealing algorithm rather than a fix number
	/// but... anyway ... who cares ....
	/// </summary>
	static double _step = 1; //NOTE: IronScheme doesn't accept float type;

	static BPNN(){
		IronScheme.RuntimeExtensions.Eval (@"
			(define step {0})

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
			            (let*
			                ([outputs_i (sim-layer (car nn) inputs)]
			                 [tmp (train-nn (cdr nn) targets outputs_i)]
			                 [delta_i+1 (car tmp)]
			                 [new-nn (cdr tmp)]
			                 [delta_i   (delta_i+1->delta_i (cadr nn) delta_i+1 outputs_i)]
			                 [new-layer (update-layer (car nn) delta_i inputs)])
			              (cons delta_i (cons new-layer new-nn)))
			            ;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
			            (let* ([outputs (map (lambda (x) (sum (map * x inputs))) (car nn))]
			                   [error (map - targets outputs)]
			                   [new-layer (update-layer (car nn) error inputs)]) ;output layer, with linear activate function
			              ;(display error)(newline)
			              (cons error (cons new-layer '())))))
			      inputs))
		",_step);
	}
	public BPNN(params object[] structure) :  base(structure){
		 
	}

	/// <summary>
	/// targets data and inputs should match the outputs and inputs of the nn
	/// that's all.
	/// </summary>
	/// <param name="targets">Targets.</param>
	/// <param name="inputs">Inputs.</param>
	public void Train(object[] targets, object[] inputs)
	{
		//Debug.Log (IronScheme.RuntimeExtensions.Eval("(cadr (vector->list {0}))",(object)inputs).ToString());
		if(inputs.Length != _n_in)
			Debug.LogError("BPNN.Train: targets length doesn't match nn outputs");
		if(inputs.Length != _n_in)
			Debug.LogError("BPNN.Train: inputs length doesn't match nn inputs");
		_nn = IronScheme.RuntimeExtensions.Eval (@"
			(cdr (train-nn {0} (vector->list {1}) (vector->list {2})))
		",_nn,((object)targets),((object)inputs));
	}

}
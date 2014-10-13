using UnityEngine;
using System.Collections;
using IronScheme;

/// <summary>
/// Example code:
/// NN n = new NN(4,3,2);
/// float out[] = n.Sim(1,2,3,4);
/// </summary>
public class NN : IronSchemeGlobalDefine  {
	protected object _nn;
	protected int _n_in=0;
	protected int _n_out=0;
	public object NNet{
		get{return _nn;}
		set{ _nn = value;}
	}
	static NN()
	{
		IronScheme.RuntimeExtensions.Eval (@"
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
		");
	}
	protected void NewNN(params object[] structure)
	{
		if (structure.Length < 1)
			Debug.LogError ("NN.NewNN: the lengh of structure should be at least 1 with positive integer number");
		_n_in = (int)structure[0];
		_n_out = (int)structure[structure.Length-1];
		_nn = IronScheme.RuntimeExtensions.Eval (@"
			(new-nn 
				;should be using random number to ini the neuro weights, 
				;somehow I couldn't find the random function in scheme yet
				(let ((a 0)) (lambda () (set! a (+ a 0.1)) a))	
				(vector->list {0}))
			", ((object)structure));
	}
	// Use this for initialization
	/// <summary>
	/// 
	/// for example: new NN(4,6,7,1); 
	/// creat a 4 input 1 outputs and 2 hidden layer with 6 and 7 neuros respectively
	/// </summary>
	/// <param name="structure">structure = [number_of_input,number_of_neuro_on_each_layer,......,number_of_outputs]</param>
	public NN(params object[] structure){
		NewNN (structure);
	}
	/// <summary>
	/// In case of unexpected call
	/// </summary>
	private NN(){

	}
	/// <summary>
	/// 
	/// </summary>
	/// <returns>The outputs of the NN</returns>
	/// <param name="inputs">Must match the input number of the NN, any numeric is accepted</param>
	public float[] SimNN(params object[] inputs)
	{
		if (_n_in != inputs.Length)
			Debug.LogError ("NN.SimNN: the length of the input doesn't match the NN.");
		object[] _outputs = (object[])IronScheme.RuntimeExtensions.Eval (@"
			(list->vector (sim-nn {0} (vector->list {1})))
		", _nn, (inputs));
		float[] outputs = new float[_outputs.Length];
		for (var j=0; j<outputs.Length; j++)
			outputs [j] = (float)(double)(_outputs [j]);//what the heck
		return outputs;
	}
}
 
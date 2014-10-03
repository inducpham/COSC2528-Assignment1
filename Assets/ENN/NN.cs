using UnityEngine;
using System.Collections;
using IronScheme;

/// <summary>
/// Example code:
/// NN n = new NN(4,3,2);
/// float out[] = n.Sim(1,2,3,4);
/// output should be 0
/// you can modify iniVal to set non-0 initial weights to see if the nn work well
/// </summary>
public class NN   {

	object _nn;
	float iniVal=0.0f;
	int n_inputs=0;
	static NN()
	{
		IronScheme.RuntimeExtensions.Eval (@"
			(define act-fun atan);activate function

			(define (create-list-by-len len value-generator)
			  (if (= len 0) '()
			      (cons (value-generator) (create-list-by-len (- len 1) value-generator))))

			(define (sim-neuro neuro inputs)
			  (act-fun (apply + (map * neuro (cons 1 inputs)))))

			(define (sim-layer layer inputs)
			  (map (lambda (n) (sim-neuro n inputs)) layer))

			(define (sim-nn nn inputs)
			  (if (pair? nn)
			      (if (pair? (cdr nn))
			          (sim-nn (cdr nn) (sim-layer (car nn) inputs)) ;input to the rest of the layers
			          (map * (car nn) inputs));output layer
			      inputs))

			(define (new-nn weigh-generator structure)
			  (if (pair? structure)
			      (if (pair? (cdr structure))
			          (let ((gen-neuro (lambda () (create-list-by-len (+ 1 (car structure)) weigh-generator))))
			            (let ((gen-layer (lambda () (create-list-by-len (cadr structure) gen-neuro))))
			              (cons (gen-layer) (new-nn weigh-generator (cdr structure)))))
			          (list (create-list-by-len (car structure) weigh-generator)))
      				'()))
		");
	}
	void NewNN(params object[] structure)
	{
		n_inputs = (int)structure[0];
		object s = structure;//in case of Eval treat 
		_nn = IronScheme.RuntimeExtensions.Eval (@"
			(new-nn 
				(lambda(){1})	;zero as the default of each weigh
				(vector->list {0}))
			", s, iniVal);
	}
	// Use this for initialization
	/// <summary>
	/// 
	/// for example: new BPNN(4,6,7,1); 
	/// creat a 4 input 1 outputs and 2 hidden layer wit 6 and 7 neuros
	/// </summary>
	/// <param name="structure">structure = [number_of_input,number_of_neuro_on_each_layer,......,number_of_outputs]</param>
	public NN(params object[] structure){
		NewNN (structure);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns>The outputs of the NN</returns>
	/// <param name="inputs">Must match the input number of the NN, any numeric is accepted</param>
	public float[] SimNN(params object[] inputs)
	{
		if (n_inputs != inputs.Length)
			Debug.LogError ("BPNN.SimNN: the length of the input doesn't match the NN.");
		object[] _outputs = (object[])IronScheme.RuntimeExtensions.Eval (@"
			(list->vector (sim-nn {0} (vector->list {1})))
		", _nn, (inputs));
		float[] outputs = new float[_outputs.Length];
		for (var j=0; j<outputs.Length; j++)
			outputs [j] = (float)(double)(_outputs [j]);//what the heck
		return outputs;
	}
}
 
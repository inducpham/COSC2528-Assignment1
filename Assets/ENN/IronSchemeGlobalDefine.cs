using UnityEngine;
using System.Collections;
using IronScheme;

public abstract class IronSchemeGlobalDefine : MonoBehaviour {

	public static double scheme_randf(double a,double b){
		return Random.Range ((float)a, (float)b);
	}
	public static int scheme_randi(int a,int b){
		return Random.Range (a, b);
	}
	static IronSchemeGlobalDefine(){
		IronScheme.RuntimeExtensions.Eval (@"
			(import  (ironscheme clr))
			(define (randi a b) (clr-static-call IronSchemeGlobalDefine scheme_randi a b))
			(define (randf a b) (clr-static-call IronSchemeGlobalDefine scheme_randf a b))
			(define (sum x) (apply + x))
			(define-syntax when
			  (syntax-rules ()
			   [(_ e rest ...)
			    (if e (begin rest ...))]))
			(define-syntax unless
			  (syntax-rules ()
			   [(_ e rest ...)
			    (if (not e) (begin rest ...))]))
			(define-syntax inc!
			  (syntax-rules ()
			    [(_ a) (set! a (+ a 1))]))
");
	}
}

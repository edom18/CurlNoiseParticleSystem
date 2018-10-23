using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CurlNoiseParticle.Demo
{
	public class Rotater : MonoBehaviour
	{
		[SerializeField]
		private float _raduis = 1f;

		[SerializeField]
		private float _speed = 0.1f;

		private Vector3 _initPos;
		private float _rad = 0;

		private void Awake()
		{
			_initPos = transform.position;
		}

		private void Update()
		{
			Rotate();
		}

		private void Rotate()
		{
			_rad += _speed;
			float x = Mathf.Cos(_rad) * _raduis;
			float z = Mathf.Sin(_rad) * _raduis;

			transform.position = _initPos + new Vector3(x, 0, z);
		}
	}
}
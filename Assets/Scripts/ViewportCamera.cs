using System;
using CodeHelpers;
using CodeHelpers.Unity;
using CodeHelpers.Vectors;
using UnityEngine;
using UnityEngine.Assertions;

namespace BlueWire
{
	public class ViewportCamera : MonoBehaviour
	{
		void Awake()
		{
			sourceCamera = GetComponent<Camera>();
			Assert.IsNotNull(sourceCamera);
		}

		[SerializeField] float movementSpeed = 7f;
		[SerializeField] float elevationSpeed = 2f;

		Camera sourceCamera;

		public Float2 WorldSize => sourceCamera.orthographicSize * 2f * new Float2(sourceCamera.aspect, 1f);

		public Float2 Position { get; private set; }
		public float Elevation { get; private set; } = 1f;

		public event Action OnReoriented;

		static readonly MinMax elevationRange = new MinMax(0.8f, 1000f);

		void Start()
		{
			OnReoriented?.Invoke();
		}

		void Update()
		{
			Int2 movementInput = InputHelper.GetWASDMovement();
			int elevationInput = Convert.ToInt32(Input.GetKey(KeyCode.E)) - Convert.ToInt32(Input.GetKey(KeyCode.Q));

			if (movementInput == Int2.zero && elevationInput == 0) return;

			Elevation = elevationRange.Clamp(Elevation + elevationInput * elevationSpeed * Time.deltaTime);
			Position += movementInput * Elevation * movementSpeed * Time.deltaTime;

			OnReoriented?.Invoke();
		}
	}
}
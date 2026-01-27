using Android.Content;
using Android.Hardware;
using Android.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthSync.Services;

namespace HealthSync.Platforms.Android
{
    public class StepCounterService : Java.Lang.Object, ISensorEventListener
    {
        SensorManager _sensorManager;
        Sensor _stepCounterSensor;

        public static StepCounterService? Instance { get; private set; }

        public event Action<int>? StepsChanged;

        int _initialStepCount = -1;
        int _lastTotalSteps = 0;

        public StepCounterService(Context context)
        {
            Instance = this;
            _sensorManager = (SensorManager)context.GetSystemService(Context.SensorService)!;
            _stepCounterSensor = _sensorManager.GetDefaultSensor(SensorType.StepCounter);

            CounterResetService.ResetStepsRequested += OnResetStepsRequested;
        }

        private void OnResetStepsRequested()
        {
            ResetBaseline();
            StepsChanged?.Invoke(0);
        }

        public void Start()
        {
            if (_stepCounterSensor != null)
            {
                _sensorManager.RegisterListener(
                    this,
                    _stepCounterSensor,
                    SensorDelay.Normal
                );
            }
        }

        public void Stop()
        {
            _sensorManager.UnregisterListener(this);
        }

        public void OnSensorChanged(SensorEvent e)
        {
            if (e.Sensor.Type != SensorType.StepCounter)
                return;

            int totalSteps = (int)e.Values[0];
            _lastTotalSteps = totalSteps;

            if (_initialStepCount < 0)
                _initialStepCount = totalSteps;

            int currentSteps = totalSteps - _initialStepCount;
            if (currentSteps < 0) currentSteps = 0;

            StepsChanged?.Invoke(currentSteps);
        }

        public void ResetBaseline()
        {
            if (_lastTotalSteps > 0)
            {
                _initialStepCount = _lastTotalSteps;
            }
            else
            {
                _initialStepCount = 0;
            }
        }

        void ISensorEventListener.OnAccuracyChanged(Sensor? sensor, SensorStatus accuracy)
        {
            throw new NotImplementedException();
        }

        void ISensorEventListener.OnSensorChanged(SensorEvent? e)
        {
            throw new NotImplementedException();
        }
    }
}
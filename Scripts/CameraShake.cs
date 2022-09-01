using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Cinemachine;
public class CameraShake : MonoBehaviour
{
    public float shakeTimer = 0f;
    public float duration = 2f;
    public float amplitudeGain = 1.2f;
    public float frequencyGain = 2.0f;
    Vector3 originalPos;
    bool CameraShaking;

    public CinemachineStateDrivenCamera cam;
    public CinemachineVirtualCamera vc;
    public CinemachineVirtualCamera basicVc;
    private CinemachineBasicMultiChannelPerlin basicVcn;
    private CinemachineBasicMultiChannelPerlin vcn;

    void Start()
    {
        if (vc != null)
            vcn = vc.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        if (basicVc != null)
            basicVcn = basicVc.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void ShakeCamera()
    {
        StartCoroutine(CoShakeCamera());
    }

    private IEnumerator CoShakeCamera()
    {
        shakeTimer = duration;
        while (shakeTimer > 0)
        {
            if (vcn != null)
            {


                if (shakeTimer > 0)
                {
                    vcn.m_AmplitudeGain = amplitudeGain;
                    vcn.m_FrequencyGain = frequencyGain;

                    shakeTimer -= Time.deltaTime;
                }

                yield return new WaitForEndOfFrame();
            }
        }


        shakeTimer = 0f;
        vcn.m_AmplitudeGain = 0;
        vcn.m_FrequencyGain = 1;
    }

    public IEnumerator CoDamageShake(float _duration, float _amplitude, float _frequency)
    {
        float  _shakeTimer = _duration;
        while (_shakeTimer > 0)
        {
            if (basicVcn != null)
            {
                if (_shakeTimer > 0)
                {
                    basicVcn.m_AmplitudeGain = _amplitude;
                    basicVcn.m_FrequencyGain = _frequency;

                    _shakeTimer -= Time.deltaTime;
                }

                yield return new WaitForEndOfFrame();
            }
        }

        basicVcn.m_AmplitudeGain = 0;
        basicVcn.m_FrequencyGain = 1;
    }

}

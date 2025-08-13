using System.Collections.Generic;
using System.IO;
using VRCFaceTracking;
using VRCFaceTracking.Core.Library;
using PSVR2Toolkit.CAPI;
using System.Threading.Tasks;
using VRCFT_Vector2 = VRCFaceTracking.Core.Types.Vector2;

namespace PSVR2Toolkit.VRCFT {
    public unsafe class TrackingModule : ExtTrackingModule {

        private bool m_eyeAvailable = false;

        private const int k_noiseFilterSamples = 15;
        private LowPassFilter? m_leftEyeOpenLowPass;
        private LowPassFilter? m_rightEyeOpenLowPass;

        public override (bool SupportsEye, bool SupportsExpression) Supported => (true, false);

        public override (bool eyeSuccess, bool expressionSuccess) Initialize(bool eyeAvailable, bool expressionAvailable) {
            ModuleInformation.Name = "PlayStation VR2 Module";

            var stream = GetType().Assembly.GetManifestResourceStream("PSVR2Toolkit.VRCFT.Resources.Logo256.png");
            if ( stream != null ) {
                ModuleInformation.StaticImages = new List<Stream> { stream };
            }

            m_leftEyeOpenLowPass = new LowPassFilter(k_noiseFilterSamples);
            m_rightEyeOpenLowPass = new LowPassFilter(k_noiseFilterSamples);

            if (IpcClient.Instance().Start()) {
                m_eyeAvailable = eyeAvailable;
            }

            return (m_eyeAvailable, false);
        }

        public override void Teardown() {
            IpcClient.Instance().Stop();
        }

        public override void Update() {
            if ( Status == ModuleState.Active ) {
                var eyeTrackingData = Task.Run(() => IpcClient.Instance().RequestEyeTrackingData()).GetAwaiter().GetResult();

                if ( eyeTrackingData.left.blinkValid ) {
                    float leftOpenness = eyeTrackingData.left.isBlinking ? 0 : 1;
                    if ( m_leftEyeOpenLowPass != null ) {
                        leftOpenness = m_leftEyeOpenLowPass.FilterValue(leftOpenness);
                    }
                    UnifiedTracking.Data.Eye.Left.Openness = leftOpenness;
                }
                if ( eyeTrackingData.right.blinkValid ) {
                    float rightOpenness = eyeTrackingData.right.isBlinking ? 0 : 1;
                    if ( m_rightEyeOpenLowPass != null ) {
                        rightOpenness = m_rightEyeOpenLowPass.FilterValue(rightOpenness);
                    }
                    UnifiedTracking.Data.Eye.Right.Openness = rightOpenness;
                }

                if ( eyeTrackingData.left.gazeValid ) {
                    UnifiedTracking.Data.Eye.Left.Gaze = new VRCFT_Vector2(eyeTrackingData.left.gaze.x, eyeTrackingData.left.gaze.y).FlipXCoordinates();
                }
                if ( eyeTrackingData.right.gazeValid ) {
                    UnifiedTracking.Data.Eye.Right.Gaze = new VRCFT_Vector2(eyeTrackingData.right.gaze.x, eyeTrackingData.right.gaze.y).FlipXCoordinates();
                }

                // pupil dilation
                if ( eyeTrackingData.left.dilationValid ) {
                    UnifiedTracking.Data.Eye.Left.PupilDiameter_MM = eyeTrackingData.left.dilation;
                }
                if ( eyeTrackingData.right.dilationValid ) {
                    UnifiedTracking.Data.Eye.Right.PupilDiameter_MM = eyeTrackingData.right.dilation;
                }

                // Force the normalization values of Dilation to fit avg. pupil values.
                UnifiedTracking.Data.Eye._minDilation = 0;
                UnifiedTracking.Data.Eye._maxDilation = 10;
            }
        }
    }
}

using System.Runtime.InteropServices;

namespace PSVR2Toolkit {
    public struct Vector3 {
        public float x, y, z;
    }
    public struct Quaternion {
        public float w, x, y, z;
    }

    public struct Psvr2Eye {
        public Vector3 gaze;
        public bool gazeValid;
        public bool isBlinking;
        public bool blinkValid;
        public float dilation;
        public bool dilationValid;
        public bool valid;
    }

    public struct Psvr2EyeTrackingData {
        public Psvr2Eye left;
        public Psvr2Eye right;
    }
}

namespace PSVR2Toolkit.CAPI {
    public enum ECommandType : ushort {
        ClientPing, // No command data.
        ServerPong, // No command data.

        ClientRequestHandshake, // CommandDataClientRequestHandshake
        ServerHandshakeResult, // CommandDataServerHandshakeResult

        ClientRequestGazeData, // No command data.
        ServerGazeDataResult, // CommandDataServerGazeDataResult

        ClientTriggerEffectOff, // CommandDataClientTriggerEffectOff
        ClientTriggerEffectFeedback, // CommandDataClientTriggerEffectFeedback
        ClientTriggerEffectWeapon, // CommandDataClientTriggerEffectWeapon
        ClientTriggerEffectVibration, // CommandDataClientTriggerEffectVibration
        ClientTriggerEffectMultiplePositionFeedback, // CommandDataClientTriggerEffectMultiplePositionFeedback
        ClientTriggerEffectSlopeFeedback, // CommandDataClientTriggerEffectSlopeFeedback
        ClientTriggerEffectMultiplePositionVibration, // CommandDataClientTriggerEffectMultiplePositionVibration
    };

    public enum EHandshakeResult : byte {
        Failed,
        Success,
        Outdated,
    };

    public enum EVRControllerType : byte {
        Left,
        Right,
        Both,
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct CommandDataClientRequestHandshake {
        public ushort ipcVersion; // The IPC version this client is using.
        public uint processId;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct CommandDataServerHandshakeResult {
        public EHandshakeResult result;
        public ushort ipcVersion; // The IPC version the server is using.
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct CommandDataServerGazeDataResult {
        // TODO
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct CommandHeader {
        public ECommandType type;
        public int dataLen;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct CommandDataClientTriggerEffectOff {
        public EVRControllerType controllerType;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct CommandDataClientTriggerEffectFeedback {
        public EVRControllerType controllerType;
        public byte position;
        public byte strength;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct CommandDataClientTriggerEffectWeapon {
        public EVRControllerType controllerType;
        public byte startPosition;
        public byte endPosition;
        public byte strength;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct CommandDataClientTriggerEffectVibration {
        public EVRControllerType controllerType;
        public byte position;
        public byte amplitude;
        public byte frequency;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct CommandDataClientTriggerEffectMultiplePositionFeedback {
        public EVRControllerType controllerType;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] strength;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct CommandDataClientTriggerEffectSlopeFeedback {
        public EVRControllerType controllerType;
        public byte startPosition;
        public byte endPosition;
        public byte startStrength;
        public byte endStrength;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct CommandDataClientTriggerEffectMultiplePositionVibration {
        public EVRControllerType controllerType;
        public byte frequency;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] amplitude;
    };


}

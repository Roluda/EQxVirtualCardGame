public static class Logger {
    public static void Log(string logMsg) {
#if UNITY_EDITOR
        UnityEngine.Debug.Log(logMsg);
#endif
    }

}

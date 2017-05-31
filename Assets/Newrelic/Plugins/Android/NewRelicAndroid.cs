#if UNITY_ANDROID

using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class NewRelicAndroid : NewRelic {

	private AndroidJavaObject activityContext = null;
	private AndroidJavaObject pluginInstance = null;
	private AndroidJavaObject agentInstance = null;
	private AndroidJavaClass unityApiClass = null;
	
	public NewRelicAndroid(NewRelicAgent plugin) : base(plugin) {

		using (AndroidJavaClass activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
			activityContext = activityClass.GetStatic<AndroidJavaObject> ("currentActivity");
			if (activityContext == null) {
				UnityEngine.Debug.LogError ("NewRelicAndroid: Could not load activity context.");
			}

			pluginInstance = new AndroidJavaClass ("com.newrelic.agent.android.NewRelic");
			if (pluginInstance == null) {
				UnityEngine.Debug.LogError ("NewRelicAndroid: Could not instantiate NewRelic plugin class.");
			}

			unityApiClass = new AndroidJavaClass ("com.newrelic.agent.android.unity.NewRelicUnity");
			if( unityApiClass == null ) {
				UnityEngine.Debug.LogError ("NewRelicAndroid: unable to instantiate a NewRelicUnity class.");
			}
		}
	}

	protected bool isValid() {
		return (pluginInstance != null);
	}

	protected bool initialize(string applicationToken) {
		if (isValid ()) {
			agentInstance = pluginInstance.CallStatic<AndroidJavaObject> ("withApplicationToken", applicationToken);
			if (agentInstance == null) {
				UnityEngine.Debug.LogError ("NewRelicAndroid: NewRelic plugin initialization failed: could not instantiate an agent instance.");
			}
		}
		return (agentInstance != null);
	}


	protected AndroidJavaObject dictionaryToHashMap( Dictionary<string, object> dictionary) {
		// Convert the C# Dictionary<string,object> to a Java Map<String,Object>
		AndroidJavaObject mapInstance = new AndroidJavaClass ("java.util.HashMap");
		if (mapInstance == null) {
			UnityEngine.Debug.LogError ("NewRelicAndroid: Could not instantiate HashMap class.");
		}

		// Call 'put' via the JNI instead of using helper classes to avoid:
		//  "JNI: Init'd AndroidJavaObject with null ptr!"
		IntPtr putMethod = AndroidJNIHelper.GetMethodID(mapInstance.GetRawClass(), "put",
		                                                 "(Ljava/lang/Object;Ljava/lang/Object;)Ljava/lang/Object;");
		object[] putCallArgs = new object[2];

		if( dictionary != null) {
			foreach (KeyValuePair<string, object> kvp in dictionary) {
				var value = kvp.Value;	// kvp.Value is read-only
			
				if (!(value is string || kvp.Value is double)) {
					try {
						value = Double.Parse (value.ToString ());
					} catch (Exception e) {
						UnityEngine.Debug.LogErrorFormat ("Coercion from [{0}] to [{1}] failed: {2}", 
							kvp.Key.GetType (), Double.MaxValue.GetType (), e.Message);
					}
				}

				if (value is string || value is double) {
					using (AndroidJavaObject k = new AndroidJavaObject ("java.lang.String", (object)kvp.Key)) {
						if (value is string) {
							using (AndroidJavaObject v = new AndroidJavaObject ("java.lang.String", (object)value)) {
								putCallArgs [0] = k;
								putCallArgs [1] = v;
								AndroidJNI.CallObjectMethod (mapInstance.GetRawObject (),
									putMethod, AndroidJNIHelper.CreateJNIArgArray (putCallArgs));
							}
						} else {
							using (AndroidJavaObject v = new AndroidJavaObject ("java.lang.Double", (object)value)) {
								putCallArgs [0] = k;
								putCallArgs [1] = v;
								AndroidJNI.CallObjectMethod (mapInstance.GetRawObject (),
									putMethod, AndroidJNIHelper.CreateJNIArgArray (putCallArgs));
							}
						}
					}
				} else {
					UnityEngine.Debug.LogError ("NewRelicAndroid: Unsupported type - value must be either string or double: " + kvp.Value);
					return null;
				}
			}
		}

		return mapInstance;
	}

	private void withBuilderMethods(AndroidJavaObject agentInstance) {

		UnityEngine.Debug.Log ("Initialize Android builder methods");

		UnityEngine.Debug.Log ("NewRelic.usingSsl: " + plugin.usingSSL.ToString ());
		agentInstance.Call<AndroidJavaObject> ("usingSsl", plugin.usingSSL);

		bool loggingEnabled = plugin.logLevel != NewRelicAgent.AgentLogLevel.NONE;

		agentInstance.Call<AndroidJavaObject> ("withLoggingEnabled", loggingEnabled);
		if (loggingEnabled) {
			UnityEngine.Debug.Log ("NewRelic.withLogLevel: " + ((int) plugin.logLevel).ToString ());
			agentInstance.Call<AndroidJavaObject> ("withLogLevel", (int) plugin.logLevel);
		}

		if (plugin.applicationVersion.Length > 0) {
			UnityEngine.Debug.Log ("NewRelic.withApplicationVersion: " + plugin.applicationVersion);
			agentInstance.Call<AndroidJavaObject> ("withApplicationVersion", plugin.applicationVersion);
		}

		if (plugin.applicationBuild.Length > 0) {
			UnityEngine.Debug.Log ("NewRelic.withApplicationBuild: " + plugin.applicationBuild);
			agentInstance.Call<AndroidJavaObject> ("withApplicationBuild", plugin.applicationBuild);
		}

		agentInstance.Call<AndroidJavaObject> ("withCrashReportingEnabled", plugin.crashReporting);
		if (plugin.crashReporting) {
			if (crashCollectorAddress.Length > 0) {
				UnityEngine.Debug.Log ("NewRelic.usingCrashCollectorAddress: " + crashCollectorAddress);
				agentInstance.Call<AndroidJavaObject> ("usingCrashCollectorAddress", crashCollectorAddress);
			}
		}

		UnityEngine.Debug.Log ("NewRelic.withHttpResponseBodyCaptureEnabled: " + plugin.httpResponseBodyCapture.ToString ());
		agentInstance.Call<AndroidJavaObject> ("withHttpResponseBodyCaptureEnabled", plugin.httpResponseBodyCapture);

		UnityEngine.Debug.Log ("NewRelic.withAnalyticsEvents: " + plugin.analyticsEvents.ToString ());
		agentInstance.Call<AndroidJavaObject> ("withAnalyticsEvents", plugin.analyticsEvents);

		if (collectorAddress.Length > 0) {
			UnityEngine.Debug.Log ("NewRelic.usingCollectorAddress: " + collectorAddress);
			agentInstance.Call<AndroidJavaObject> ("usingCollectorAddress", collectorAddress);
		}
	}

	protected void setPlatform() {
		// set the platform type and version
		using (AndroidJavaObject platform = new AndroidJavaClass ("com.newrelic.agent.android.ApplicationPlatform").GetStatic<AndroidJavaObject> ("Unity")) {
			agentInstance.Call<AndroidJavaObject> ("withApplicationFramework", platform);
			using (AndroidJavaObject agentConfig = pluginInstance.GetStatic<AndroidJavaObject> ("agentConfiguration")) {
				agentConfig.Call ("setApplicationPlatformVersion", platformVersion);
				UnityEngine.Debug.Log ("Unity application platform: version  " + platformVersion);
			}
		}
	}

	override public void start(string applicationToken) {
		if( initialize (applicationToken) ) {
			// set the platform type and version
			setPlatform ();

			// Call the NewRelic builder methods. Must be call *before** start() method
			withBuilderMethods(agentInstance);

			// finally, start the agent
			agentInstance.Call ("start", this.activityContext);
		} else {
			UnityEngine.Debug.LogError ("NewRelicAndroid: NewRelic plugin initialization failed: no instance");
		}
	}

	override public void crashNow(string message) {
		throw new SystemException (message);
	}


	// Configuration

	override public void enableFeatures(int features) {
		if (NewRelicAgent.FeatureFlag.HttpResponseBodyCapture.Equals((features & (int) NewRelicAgent.FeatureFlag.HttpResponseBodyCapture))) {
			UnityEngine.Debug.Log ("NewRelic.enableFeatures: HttpResponseBodyCapture");
			agentInstance.Call ("withHttpResponseBodyCaptureEnabled", true);
		}

		if (NewRelicAgent.FeatureFlag.CrashReporting.Equals ((features & (int) NewRelicAgent.FeatureFlag.CrashReporting))) {
			UnityEngine.Debug.Log ("NewRelic.enableFeatures: CrashReporting");
			agentInstance.Call ("withCrashReportingEnabled", true);
		}

		if (NewRelicAgent.FeatureFlag.AnalyticsEvents.Equals((features & (int) NewRelicAgent.FeatureFlag.AnalyticsEvents))) {
			UnityEngine.Debug.Log ("NewRelic.enableFeatures: AnalyticsEvents");
			agentInstance.Call ("withAnalyticsEvents", true);
		}

	}

	override public void disableFeatures(int features) {
		if (NewRelicAgent.FeatureFlag.HttpResponseBodyCapture.Equals((features & (int) NewRelicAgent.FeatureFlag.HttpResponseBodyCapture))) {
			UnityEngine.Debug.Log ("NewRelic.disableFeatures: HttpResponseBodyCapture");
			agentInstance.Call ("withHttpResponseBodyCaptureEnabled", false);
		}

		if (NewRelicAgent.FeatureFlag.CrashReporting.Equals ((features & (int) NewRelicAgent.FeatureFlag.CrashReporting))) {
			UnityEngine.Debug.Log ("NewRelic.CrashReporting: HttpResponseBodyCapture");
			agentInstance.Call ("withCrashReportingEnabled", false);
		}

		if (NewRelicAgent.FeatureFlag.AnalyticsEvents.Equals((features & (int) NewRelicAgent.FeatureFlag.AnalyticsEvents))) {
			UnityEngine.Debug.Log ("NewRelic.AnalyticsEvents: HttpResponseBodyCapture");
			agentInstance.Call ("withAnalyticsEvents", false);
		}
	}

	override public void enableCrashReporting(bool enabled) {
		UnityEngine.Debug.Log ("NewRelic.enableCrashReporting: " + enabled.ToString());
		pluginInstance.Call<AndroidJavaObject> ("withCrashReportingEnabled", enabled);
	}

	override public void setApplicationVersion(string version) {
		UnityEngine.Debug.Log ("NewRelic.setApplicationVersion: " + version);
		agentInstance.Call<AndroidJavaObject> ("withApplicationVersion", version);
	}

	override public void setApplicationBuild(string buildNumber) {
		UnityEngine.Debug.Log ("NewRelic.setApplicationBuild: " + buildNumber);
		agentInstance.Call<AndroidJavaObject> ("withApplicationBuild", buildNumber);
	}

	override public string currentSessionId() {
		UnityEngine.Debug.Log ("NewRelic.currentSessionId: ");
		return pluginInstance.CallStatic<string> ("currentSessionId");
	}


	// Custom Instrumentation

	override public string startInteractionWithName(string name) {
		string interactionTraceId = pluginInstance.CallStatic<string> ("startInteraction", name);
		return interactionTraceId;
	}

	override public void stopCurrentInteraction(string interactionIdentifier) {
		pluginInstance.CallStatic ("endInteraction", interactionIdentifier);
	}

	override public void startTracingMethod(string methodName, string className, Timer timer, NewRelicAgent.NRTraceType category) {
		using (AndroidJavaClass traceMachineClass = new AndroidJavaClass("com.newrelic.agent.android.tracing.TraceMachine")) {
			traceMachineClass.CallStatic ("enterMethod", className + "." + methodName);
		}
	}

	override public void endTracingMethodWithTimer (Timer timer) {
		using (AndroidJavaClass traceMachineClass = new AndroidJavaClass("com.newrelic.agent.android.tracing.TraceMachine")) {
			traceMachineClass.CallStatic ("exitMethod");
		}
	}


	// Metrics

	private Dictionary<string, AndroidJavaObject> metricUnitCache = new Dictionary<string, AndroidJavaObject>();

	private AndroidJavaObject metricUnitToEnum(NewRelicAgent.MetricUnit mu) {
		AndroidJavaObject metricUnit = null;
		string unit = mu.ToString ();

		if (unit != null) {
			if (metricUnitCache.ContainsKey (unit)) {
				metricUnit = metricUnitCache [unit];
			} else {
				try {
					metricUnit = new AndroidJavaClass ("com.newrelic.agent.android.metric.MetricUnit").GetStatic<AndroidJavaObject> (unit);
				} catch (AndroidJavaException) {
					UnityEngine.Debug.Log ("NewRelicAgent.metricUnitToEnum: invalid MetricUnit passed [" + unit + "]");
				}
				metricUnitCache.Add (unit, metricUnit);
			}
		}
		return metricUnit;
	}

	override public void recordMetricWithName(string name, string category) {
		pluginInstance.CallStatic ("recordMetric", name, category);
	}

	override public void recordMetricWithName(string name, string category, double value) {
		pluginInstance.CallStatic ("recordMetric", name, category, value);
	}

	override public void recordMetricWithName(string name, string category, double value, NewRelicAgent.MetricUnit valueUnits) {
		AndroidJavaObject metricUnit = metricUnitToEnum (valueUnits);
		pluginInstance.CallStatic ("recordMetric", name, category, 1, value, value, metricUnit, metricUnit);
	}

	override public void recordMetricWithName(string name,
	                                 string category,
	                                 double value,
									 NewRelicAgent.MetricUnit valueUnits,
									 NewRelicAgent.MetricUnit countUnits) {
		pluginInstance.CallStatic ("recordMetric", name, category, 1, value, value, 
			metricUnitToEnum(valueUnits), metricUnitToEnum(countUnits));
	}

	private long dateTimeToMillisSinceEpoch(DateTime dateTime) {
		TimeSpan span = (dateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
		return (long)span.TotalMilliseconds;
	}


	// Networking

	override public void noticeNetworkRequest(string url,
	                                string httpMethod,
	                                Timer timer,
	                                Dictionary<string,object> headers,	// unused
	                                int httpStatusCode,
	                                int bytesSent,
	                                int bytesReceived,
	                                byte[] responseData,
	                                Dictionary<string,object> parameters) {
		AndroidJavaObject parametersMap = dictionaryToHashMap (parameters);
		string responseDataString = System.Text.Encoding.UTF8.GetString(responseData);

		pluginInstance.CallStatic ("noticeHttpTransaction", url, httpMethod, httpStatusCode, dateTimeToMillisSinceEpoch(timer.start), dateTimeToMillisSinceEpoch(timer.end),
		                          (long) bytesSent, (long) bytesReceived, responseDataString, parametersMap.GetRawObject());
	}

	override public void noticeNetworkFailure(string url,
	                                 string httpMethod,
	                                 Timer timer,
	                                 NewRelicAgent.NetworkFailureCode failureCode,
	                                 string message) {
		// Invoke the Android agent noticeNetworkFailure(url, httpMethod, startTime, endTime, exception)
		unityApiClass.CallStatic ("noticeNetworkFailure", url, httpMethod, dateTimeToMillisSinceEpoch(timer.start), dateTimeToMillisSinceEpoch(timer.end), (int) failureCode, message);
	}


	// Insights Events

	override public bool recordEvent(string name, Dictionary<string,object> attributes) {
		AndroidJavaObject eventName = new AndroidJavaObject ("java.lang.String", name);
		AndroidJavaObject eventInstance = new AndroidJavaObject ("com.newrelic.agent.android.unity.UnityEvent", eventName);

		object[] addAttributeCallArgs = new object[2];

		if (attributes != null) {
			foreach (KeyValuePair<string, object> kvp in attributes) {
				var value = kvp.Value;	// kvp.Value is read-only

				if (!(value is string || kvp.Value is double)) {
					try {
						value = Double.Parse (value.ToString ());
					} catch (Exception e) {
						UnityEngine.Debug.LogErrorFormat ("Coercion from [{0}] to [{1}] failed: {2}", 
							kvp.Key.GetType (), Double.MaxValue.GetType (), e.Message);
					}
				}

				if (value is string || value is double) {
					using (AndroidJavaObject k = new AndroidJavaObject ("java.lang.String", (object)kvp.Key)) {
						if (value is string) {
							using (AndroidJavaObject v = new AndroidJavaObject ("java.lang.String", (object)value)) {
								addAttributeCallArgs [0] = k;
								addAttributeCallArgs [1] = v;
								eventInstance.Call ("addAttribute", addAttributeCallArgs);
							}
						} else {
							using (AndroidJavaObject v = new AndroidJavaObject ("java.lang.Double", (object)value)) {
								addAttributeCallArgs [0] = k;
								addAttributeCallArgs [1] = v;
								eventInstance.Call ("addAttribute", addAttributeCallArgs);
							}
						}
					}
				} else {
					UnityEngine.Debug.LogErrorFormat ("NewRelicAndroid: Unsupported value type[{0}][{1}:{2}] - value must be either string or double: ",  
						kvp.Key, value, value.GetType ());
					return false;
				}
			}
		}

		return unityApiClass.CallStatic<Boolean> ("recordEvent", eventInstance);
	}

	override public void setMaxEventPoolSize(uint size) {
		UnityEngine.Debug.Log ("NewRelicAndroid.setMaxEventPoolSize: " + size);
		pluginInstance.CallStatic ("setMaxEventPoolSize", size);
	}

	override public void setMaxEventBufferTime(uint seconds) {
		UnityEngine.Debug.Log ("NewRelicAndroid.setMaxEventBufferTime: " + seconds);
		pluginInstance.CallStatic ("setMaxEventBufferTime", seconds);
	}

	override public bool setAttribute(string name, string value) {
		UnityEngine.Debug.Log ("NewRelicAndroid.setAttribute: " + name + ", string value: " + value);
		return pluginInstance.CallStatic<Boolean> ("setAttribute", name, value);
	}

	override public bool setAttribute(string name, double value) {
		UnityEngine.Debug.Log ("NewRelicAndroid.setAttribute: " + name + ", double value: " + value);
		return pluginInstance.CallStatic<Boolean> ("setAttribute", name, Convert.ToSingle(value));
	}

	override public bool incrementAttribute(string name) {
		UnityEngine.Debug.Log ("NewRelicAndroid.incrementAttribute: " + name);
		return pluginInstance.CallStatic<Boolean> ("incrementAttribute", name);
	}

	override public bool incrementAttribute(string name, double amount) {
		UnityEngine.Debug.Log ("NewRelicAndroid.incrementAttribute: " + name + ", amount: " + amount);
		return pluginInstance.CallStatic<Boolean> ("incrementAttribute", name, Convert.ToSingle(amount));
	}

	override public bool removeAttribute(string name) {
		UnityEngine.Debug.Log ("NewRelicAndroid.removeAttribute: " + name);
		return pluginInstance.CallStatic<Boolean> ("removeAttribute", name);
	}

	override public bool removeAllAttributes() {
		UnityEngine.Debug.Log ("NewRelicAndroid.removeAllAttributes");
		return pluginInstance.CallStatic<Boolean> ("removeAllAttributes");
	}

	
	private static void throwUnityException(string cause, string message, string stackTrace) {

		using (AndroidJavaObject unityException = new AndroidJavaObject("com.newrelic.agent.android.unity.UnityException", cause, message)) {
		
			Regex stackFrameRegex = new Regex ("(\\S+)\\s*\\(.*?\\)\\s*(?:(?:\\[.*\\]\\s*in\\s|\\(at\\s*\\s*)(.*):(\\d+))?",
			                                   RegexOptions.IgnoreCase | RegexOptions.Multiline);
			
			// break stack trace string down into stack frames
			foreach (Match frame in stackFrameRegex.Matches(stackTrace)) {
				string className = String.Empty;
				string methodName = String.Empty;
				string fileName = String.Empty;
				string lineNumber = "0";

				if (frame.Groups.Count > 1) {
					methodName = frame.Groups [1].Value;

					if (!String.IsNullOrEmpty (methodName)) {
						string [] splits = methodName.Split (".".ToCharArray ());
						if (splits != null && splits.Length > 1) {
							className = splits [0];
							methodName = splits [1];
						}
					}

					if (frame.Groups.Count > 2) {
						string filenameValue = frame.Groups [2].Value;
						if (!(String.IsNullOrEmpty (filenameValue) || 
							filenameValue.ToLower ().Equals ("<filename unknown>"))) {
							fileName = filenameValue;
						}

						if (frame.Groups.Count > 3) {	
							string lineNumberValue = frame.Groups [3].Value;
							if (!String.IsNullOrEmpty (lineNumberValue)) {
								lineNumber = lineNumberValue;
							}
						}
					}
				}

				try {
					unityException.Call ("appendStackFrame", className, methodName, fileName, Convert.ToInt32(lineNumber));
				} catch( Exception e ) {
					UnityEngine.Debug.LogError("NewRelicAndroid: appendStackFrame[" + e.Message + "]");
				}
			}

			using( AndroidJavaObject newRelicUnity = new AndroidJavaObject("com.newrelic.agent.android.unity.NewRelicUnity")) {
				try {
					newRelicUnity.CallStatic("handleUnityCrash", unityException);
				} catch( Exception e ) {
					UnityEngine.Debug.LogError("NewRelicAndroid: handleUnityCrash[" + e.Message + "]");
				}
			}
		}
	}

	public static void logMessageHandler (string logString, string stackTrace, LogType type)
	{
		if (type == LogType.Exception) {
			Regex logMessageRegex = new Regex(@"^(?<class>\S+):\s*(?<message>.*)");

			string exceptionClass = String.Empty;
			string exceptionMsg = String.Empty;
			Match match = logMessageRegex.Match(logString);

			if( match.Success ) {
				if( match.Groups.Count > 1 ) {
					exceptionClass = match.Groups["class"].Value;
					exceptionMsg = match.Groups["message"].Value;
				}
			}

			if(String.IsNullOrEmpty(stackTrace)) {
				stackTrace = new System.Diagnostics.StackTrace(1, true).ToString();
			}	

			throwUnityException(exceptionClass, exceptionMsg, stackTrace);
		}
	}
	
	public static void unhandledExceptionHandler (object sender, System.UnhandledExceptionEventArgs args)
	{
		if (args != null ) {
			if( args.ExceptionObject != null ) {
				if(args.ExceptionObject.GetType () == typeof(System.Exception)) {	
					System.Exception e = (System.Exception) args.ExceptionObject as System.Exception;
					throwUnityException(e.GetType().ToString(), e.Message, e.StackTrace);
				}
			}
		}
	}
	
}

#endif // UNITY_ANDROID

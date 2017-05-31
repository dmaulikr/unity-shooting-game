using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

#if UNITY_IPHONE
public class NewRelicIos : NewRelic {

	[DllImport ("__Internal")]
	private static extern void NR_useSSL(bool useSSL);

	[DllImport ("__Internal")]
	private static extern void NR_logLevel(int logLevel);

	[DllImport ("__Internal")]
	private static extern void NR_crashNow(string message);

	[DllImport ("__Internal")]
	private static extern void NewRelic_startWithApplicationToken(string appToken);

	[DllImport ("__Internal")]
	private static extern void NR_enableFeatures(int features);

	[DllImport ("__Internal")]
	private static extern void NR_disableFeatures(int features);

	[DllImport ("__Internal")]
	private static extern void NR_enableCrashReporting(bool enabled);

	[DllImport ("__Internal")]
	private static extern void NR_setApplicationVersion(String version);

	[DllImport ("__Internal")]
	private static extern void NR_setApplicationBuild(String buildNumber);

	[DllImport ("__Internal")]
	private static extern String NR_currentSessionId(); 

	[DllImport ("__Internal")]
	private static extern String NR_startInteractionWithName(String name);

	[DllImport ("__Internal")]
	private static extern void NR_stopCurrentInteraction(String interactionIdentifier);

	[DllImport ("__Internal")]
	private static extern void NR_startTracingMethod(String methodName, String className, System.IntPtr timer, int category);

	[DllImport ("__Internal")]
	private static extern void NR_endTracingMethodWithTimer (System.IntPtr timer);
	
	//metrics
	[DllImport ("__Internal")]
	private static extern void NR_recordMetricsWithName(String name, String category);

	[DllImport ("__Internal")]
	private static extern void NR_recordMetricsWithNameValue(String name, String category, double value);

	[DllImport ("__Internal")]
	private static extern void NR_recordMetricsWithNameValueUnits(String name, String category, double value, String valueUnits);

	[DllImport ("__Internal")]
	private static extern void NR_recordMetricsWithNameValueAndCountUnits(String name, String category, double value, String valueUnits, String countUnits);
	
	
	//Networking
	[DllImport ("__Internal")]
	private static extern void NR_noticeNetworkRequest(string URL,
	                                               	   string httpMethod,
	                                                   System.IntPtr timer,
	                                                   System.IntPtr headersNSDictionary,
	                                 			 	   int httpStatusCode,
	                                    		   	   int bytesSent,
	                                    		   	   int bytesReceived,
	                                    		   	   byte[] responseData,
	                                                   int responseDataLength,
	                                                   System.IntPtr paramsNSDictionary);
	[DllImport ("__Internal")]
	private static extern void NR_noticeNetworkFailure(string url,
	                                                   string httpMethod,
	                                                   System.IntPtr timer,
	                                                   int failureCode);

	//Insights Events

	[DllImport ("__Internal")]
	private static extern bool NR_recordEvent(string named,
	                                          System.IntPtr dictionary);

	[DllImport ("__Internal")]
	private static extern bool NR_setMaxEventPoolSize(uint size);

	[DllImport ("__Internal")]
	private static extern void NR_setMaxEventBufferTime(uint seconds);

	[DllImport ("__Internal")]
	private static extern bool NR_setAttributeStringValue(String name, String value);

	[DllImport ("__Internal")]
	private static extern bool NR_setAttributeDoubleValue(String name, double value);

	[DllImport ("__Internal")]
	private static extern bool NR_incrementAttribute(String name);

	[DllImport ("__Internal")]
	private static extern bool NR_incrementAttributeWithValue(String name, double amount);

	[DllImport ("__Internal")]
	private static extern bool NR_removeAttribute(String name);

	[DllImport ("__Internal")]
	private static extern bool NR_removeAllAttributes();
			
    [DllImport ("__Internal")]
    private static extern void NR_setPlatform(String version);
	
	public void useSSL(bool useSSL) {
		NR_useSSL(useSSL);
	}

	public NewRelicIos(NewRelicAgent plugin) : base(plugin) 
	{
		useSSL(plugin.usingSSL);
		
		if(plugin.applicationBuild != null && plugin.applicationBuild.Length > 0) {
			setApplicationBuild(plugin.applicationBuild);
		}
		
		if(plugin.applicationVersion != null && plugin.applicationVersion.Length > 0) {
			setApplicationVersion(plugin.applicationVersion);
		}

    	logLevel((int) plugin.logLevel);

		enableCrashReporting(plugin.crashReporting);
		
		if(plugin.interactionTracing) {
			enableFeatures((int)NewRelicAgent.FeatureFlag.InteractionTracing);
		} else { 
			disableFeatures((int)NewRelicAgent.FeatureFlag.InteractionTracing);
		}
		
		if(plugin.swiftInteractionTracing) {
			enableFeatures((int)NewRelicAgent.FeatureFlag.SwiftInteractionTracing);
		} else {
			disableFeatures((int)NewRelicAgent.FeatureFlag.SwiftInteractionTracing);
		}
		
		if(plugin.URLSessionInstrumentation) {
			enableFeatures((int)NewRelicAgent.FeatureFlag.URLSessionInstrumentation);
		} else {
			disableFeatures((int)NewRelicAgent.FeatureFlag.URLSessionInstrumentation);
		}
		
		
		if(plugin.httpResponseBodyCapture) {
			enableFeatures((int)NewRelicAgent.FeatureFlag.HttpResponseBodyCapture);
		} else {
			disableFeatures((int)NewRelicAgent.FeatureFlag.HttpResponseBodyCapture);
		}
		
		if(plugin.experimentalNetworkingInstrumentation) {
			enableFeatures((int)NewRelicAgent.FeatureFlag.ExperimentalNetworkingInstrumentation);
		} else {
			disableFeatures((int)NewRelicAgent.FeatureFlag.ExperimentalNetworkingInstrumentation);
		}
		
		if(plugin.analyticsEvents) {
			enableFeatures((int)NewRelicAgent.FeatureFlag.AnalyticsEvents);
		} else {
			disableFeatures((int)NewRelicAgent.FeatureFlag.AnalyticsEvents);
		}
	}

	public void logLevel(int logLevel) {
		NR_logLevel(logLevel);
	}

	override public void start(string applicationToken) {
        NR_setPlatform(platformVersion);
		NewRelic_startWithApplicationToken(applicationToken);
	}
        
	override public void crashNow(string message) {
		NR_crashNow(message);
	}


	// Configuration

	override public void enableFeatures(int features){
		NR_enableFeatures(features);
	}

	override public void disableFeatures(int features) {
		NR_disableFeatures(features);
	}

	override public void enableCrashReporting(bool enabled) {
		NR_enableCrashReporting(enabled);
	}

	override public void setApplicationVersion(string version) {
		NR_setApplicationVersion(version);
	}

	override public void setApplicationBuild(string buildNumber) {
		NR_setApplicationBuild(buildNumber);
	}

	override public string currentSessionId() {
		return NR_currentSessionId();
	}

	// Custom Instrumentation

	override public string startInteractionWithName(string name) {
		return NR_startInteractionWithName(name);
	}

	override public void stopCurrentInteraction(string interactionIdentifier) {
		NR_stopCurrentInteraction(interactionIdentifier);
	}

	override public void startTracingMethod(string methodName, string objectName, Timer timer, NewRelicAgent.NRTraceType category) {
		NR_startTracingMethod(methodName,objectName, timer.handle,(int)category);
	}

	override public void endTracingMethodWithTimer (Timer timer) {
		NR_endTracingMethodWithTimer(timer.handle);
	}

	// Metrics
	private Dictionary<NewRelicAgent.MetricUnit, string> metricUnitCache = new Dictionary<NewRelicAgent.MetricUnit, string> ();

	private string enumToMetricUnitString (NewRelicAgent.MetricUnit unit) {
		string metricUnit = null;

		if (metricUnitCache.ContainsKey (unit)) {
			metricUnit = metricUnitCache [unit];
		} else {
			switch (unit) {
			case NewRelicAgent.MetricUnit.BYTES:
				metricUnit = "bytes";
				break;
			case NewRelicAgent.MetricUnit.BYTES_PER_SECOND:
				metricUnit = "bytes/second";
				break;
			case NewRelicAgent.MetricUnit.OPERATIONS:
				metricUnit = "op";
				break;
			case NewRelicAgent.MetricUnit.PERCENT:
				metricUnit = "%";
				break;
			case NewRelicAgent.MetricUnit.SECONDS:
				metricUnit = "sec";
				break;
			}
			metricUnitCache.Add (unit, metricUnit);
		}
		return metricUnit;
	}

	override public void recordMetricWithName(string name, string category) {
		NR_recordMetricsWithName(name,category);
	}

	override public void recordMetricWithName(string name, string category, double value) {
		NR_recordMetricsWithNameValue(name,category,value);
	}

	override public void recordMetricWithName (string name, string category, double value, NewRelicAgent.MetricUnit valueUnits) {
		NR_recordMetricsWithNameValueUnits (name, category, value, enumToMetricUnitString (valueUnits));
	}

	override public void recordMetricWithName (string name, 
	                                          string category, 
	                                          double value, 
	                                          NewRelicAgent.MetricUnit valueUnits, 
	                                          NewRelicAgent.MetricUnit countUnits) {
		NR_recordMetricsWithNameValueAndCountUnits (name, category, value, enumToMetricUnitString (valueUnits), enumToMetricUnitString (countUnits));
	}

	// Networking

	override public void noticeNetworkRequest (string URL,
	                                string httpMethod,
	                                Timer timer, 
	                                Dictionary<string,object> headers,
	                                int httpStatusCode,
	                                int bytesSent,
	                                int bytesReceived,
	                                byte[] responseData,
	                                Dictionary<string,object> parameters){

		NativeDictionaryWrapper nativeHeaders = new NativeDictionaryWrapper();
		if (headers != null) {
			foreach(KeyValuePair<string,object> kvp in headers) {
				nativeHeaders.insert(kvp.Key,kvp.Value);
			}
		}
		NativeDictionaryWrapper nativeParameters = new NativeDictionaryWrapper();
		if (parameters != null) {
			foreach(KeyValuePair<string,object> kvp in parameters) {
				nativeParameters.insert(kvp.Key,kvp.Value);
			}
		}

		int responseDataLength = 0;
		if (responseData != null){
			responseDataLength = responseData.Length;
		}

		NR_noticeNetworkRequest(URL,
		                        httpMethod,
		                        timer.handle,
		                        nativeHeaders.handle,
		                        httpStatusCode,
		                        bytesSent,
		                        bytesReceived,
		                        responseData,
		                        responseDataLength,
		                        nativeParameters.handle);


	}

	override public void noticeNetworkFailure(string url,
	                                 string httpMethod,
	                                 Timer timer,
	                                 NewRelicAgent.NetworkFailureCode failureCode,
	                                 string message) {
		NR_noticeNetworkFailure(url,
		                        httpMethod,
		                        timer.handle,
		                        (int)failureCode);
	}


	// Insights Events

	override public bool recordEvent(string name,
	                        Dictionary<string,object> attributes) {
		NativeDictionaryWrapper nativeAttributes = new NativeDictionaryWrapper();
		if(attributes != null) {
			foreach (KeyValuePair<string,object> kvp in attributes) {
				if(!nativeAttributes.insert(kvp.Key,kvp.Value)) {
					UnityEngine.Debug.LogError("ERROR: NewRelic recordEvent() failed. event named '"+ name +"' failed due to invalid key-value pair in attributes: key='"+ kvp.Key +"', value='"+ kvp.Value +"'");
					return false;
				}
			}
		}
		return NR_recordEvent(name, nativeAttributes.handle);
	}

	override public void setMaxEventPoolSize(uint size) {
		NR_setMaxEventPoolSize(size);
	}
		
	override public void setMaxEventBufferTime(uint seconds) {
		NR_setMaxEventBufferTime(seconds);
	}

	override public bool setAttribute(string name, string value) {
		return NR_setAttributeStringValue(name, value);
	}

	override public bool setAttribute(string name, double value) {
		return NR_setAttributeDoubleValue(name, value);
	}

	override public bool incrementAttribute(string name) {
		return NR_incrementAttribute(name);
	}

	override public bool incrementAttribute(string name, double amount) {
		return NR_incrementAttributeWithValue(name, amount);
	}

	override public bool removeAttribute(string name) {
		return NR_removeAttribute(name);
	}

	override public bool removeAllAttributes() {
		return NR_removeAllAttributes();
	}
}

#endif

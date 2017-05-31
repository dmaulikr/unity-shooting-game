//
//  New Relic for Mobile -- iOS edition
//
//  See:
//    https://docs.newrelic.com/docs/mobile-apps for information
//    https://docs.newrelic.com/docs/releases/ios for release notes
//
//  Copyright (c) 2014 New Relic. All rights reserved.
//  See https://docs.newrelic.com/docs/licenses/ios-agent-licenses for license details
//

#import <Foundation/Foundation.h>
#import <NewRelicAgent/NewRelic.h>
#ifdef __cplusplus
extern "C" {
#endif


    extern void NR_logLevel(int logLevel);

    extern void NR_dictionaryDispose(NSMutableDictionary* dictionary);

    extern NSMutableDictionary* NR_dictionaryCreate();

    extern void NR_dictionaryInsertString(NSMutableDictionary* dictionary, const char* key, const char* value);

    extern void NR_dictionaryInsertInt64(NSMutableDictionary* dictionary, const char* key, int64_t value);

    extern void NR_dictionaryInsertUInt64(NSMutableDictionary* dictionary, const char* key, uint64_t value);

    extern void NR_dictionaryInsertDouble(NSMutableDictionary* dictionary, const char* key, double value);

    extern void NR_dictionaryInsertFloat(NSMutableDictionary* dictionary, const char* key, float value);

    extern void NewRelic_startWithApplicationToken(const char* appToken);

    extern void NR_crashNow(const char* message);
    extern void NR_enableFeatures(int features);
    extern void NR_disableFeatures(int features);
    extern void NR_enableCrashReporting(bool enabled);
    extern void NR_setApplicationVersion(const char* version);
    extern void NR_setApplicationBuild(const char* buildNumber);
    extern void NR_setPlatform(const char* version);
    extern const char* currentSessionId();


    //Interactions

    extern const char* NR_startInteractionWithName(const char* name);
    extern void NR_stopCurrentInteraction(const char* interactionIdentifier);
    extern void NR_startTracingMethod(const char* methodName, const char* className, NRTimer* tiemr, int category);
    extern void NR_endTracingMethodWithTimer(NRTimer* timer);


    //metrics
    extern void NR_recordMetricsWithName(const char* name, const char* category);
    extern void NR_recordMetricsWithNameValue(const char* name, const char* category, double value);
    extern void NR_recordMetricsWithNameValueUnits(const char* name, const char* category, double value, const char* valueUnits);
    extern void NR_recordMetricsWithNameValueAndCountUnits(const char* name, const char* category, double value, const char* valueUnits, const char* countUnits);


    //Networking

    extern void NR_noticeNetworkRequest(const char* URL,
                                        const char* httpMethod,
                                        NRTimer* timer,
                                        NSMutableDictionary* headersNSDictionary,
                                        int httpStatusCode,
                                        int bytesSent,
                                        int bytesReceived,
                                        uint8_t* responseData,
                                        int responseDataSize,
                                        NSMutableDictionary* paramsNSDictionary);


    extern void NR_noticeNetworkFailure(const char* url,
                                        const char* httpMethod,
                                        NRTimer* timer,
                                        int failureCode);

    //Insights Events

    extern bool NR_recordEvent(const char* named,
                            NSMutableDictionary* dictionary);
    extern void NR_setMaxEventPoolSize(unsigned int size);
    extern void NR_setMaxEventBufferTime(unsigned int seconds);

    extern bool NR_setAttributeStringValue(const char* name, const char* value);
    extern bool NR_setAttributeDoubleValue(const char* name, double value);
    extern bool NR_incrementAttribute(const char* name);
    extern bool NR_incrementAttributeWithValue(const char* name, double amount);

    extern bool NR_removeAttribute(const char* name);
    extern bool NR_removeAllAttributes();
    
    
    
    extern void NR_NewRelic_crash();
#ifdef __cplusplus
}
#endif

(function(){HoneywellBarcodeReaderSwiftWin=function(a,b){var e=this,c,d;a&&"string"!==typeof a?HoneywellBarcodeReaderUtils.isFunction(b)&&setTimeout(function(){b({status:HoneywellBarcodeReaderErrors.INVALID_PARAMETER,message:"Invalid parameter: scannerName, must be string"})},0):(this.scannerName=a?a:"default",c=HoneywellBarcodeReaderUtils.getRandomInt(1E4,99999),d=win10EnterpriseBrowserInfo.getBrowserPortalSeqNum(),win10ScannerBridge.openAsync(c,this.scannerName,d).then(function(a){if(a){e.logVar("win10ScannerBridge.openAsync response",
a,!1);var c;try{c=JSON.parse(a)}catch(d){HoneywellBarcodeReaderUtils.isFunction(b)&&setTimeout(function(){b({status:HoneywellBarcodeReaderErrors.JSON_PARSE_ERROR,message:"JSON-RPC parsing error in response."})},0);return}HoneywellBarcodeReaderUtils.hasProperty(c,"result",!0)?HoneywellBarcodeReaderUtils.hasProperty(c.result,"scannerHandle",!0)&&HoneywellBarcodeReaderUtils.hasProperty(c.result,"interface",!0)&&HoneywellBarcodeReaderUtils.hasProperty(c.result,"modelName",!0)?(e.scannerHandle=c.result.scannerHandle,
e.scannerInterface=c.result.interface,e.scannerModelName=c.result.modelName,win10ScannerBridge.removeEventListener("barcodedataready",window["dataReadyEventHandler"+e.scannerHandle]),e.barcodeDataReadyHandlersRegistered[e.scannerHandle]=!1,HoneywellBarcodeReaderUtils.isFunction(b)&&setTimeout(function(){b({status:0,message:e.MSG_OPERATION_COMPLETED})},0)):HoneywellBarcodeReaderUtils.isFunction(b)&&setTimeout(function(){b({status:HoneywellBarcodeReaderErrors.FUNCTION_FAILED,message:"Missing scanner handle or interface or modelName in response."})},
0):HoneywellBarcodeReaderUtils.hasJsonRpcError(c)?HoneywellBarcodeReaderUtils.isFunction(b)&&setTimeout(function(){b({status:c.error.code,message:c.error.message})},0):HoneywellBarcodeReaderUtils.isFunction(b)&&setTimeout(function(){b({status:HoneywellBarcodeReaderErrors.JSON_PARSE_ERROR,message:"JSON-RPC parsing error in response."})},0)}else HoneywellBarcodeReaderUtils.isFunction(b)&&setTimeout(function(){b({status:HoneywellBarcodeReaderErrors.FUNCTION_FAILED,message:"Null or empty response."})},
0)}))};HoneywellBarcodeReaderSwiftWin.prototype={version:"1.20.00.0825",scannerName:null,scannerHandle:null,scannerInterface:null,scannerModelName:null,barcodeDataReadyListeners:[],barcodeDataReadyHandlersRegistered:[],MSG_OPERATION_COMPLETED:"Operation completed successfully.",MSG_READER_CLOSED:"Barcode reader already closed.",batchSetBuffer:[],activate:function(a,b){var e=this,c;this.verifyActiveConnection(b)&&HoneywellBarcodeReaderUtils.stdParamCheck(a,"on","boolean",b)&&(c=HoneywellBarcodeReaderUtils.getRandomInt(1E4,
99999),win10ScannerBridge.activateAsync(c,this.scannerHandle,a).then(function(a){if(a){e.logVar("win10ScannerBridge.activateAsync response",a,!1);var c;try{c=JSON.parse(a)}catch(g){HoneywellBarcodeReaderUtils.isFunction(b)&&setTimeout(function(){b({status:HoneywellBarcodeReaderErrors.JSON_PARSE_ERROR,message:"JSON-RPC parsing error in response."})},0);return}HoneywellBarcodeReaderUtils.stdErrorCheck(c,b)}else HoneywellBarcodeReaderUtils.isFunction(b)&&setTimeout(function(){b({status:HoneywellBarcodeReaderErrors.FUNCTION_FAILED,
message:"Null or empty response."})},0)}))},addEventListener:function(a,b,e){if("barcodedataready"===a&&this.scannerHandle){var c=this,d="dataReadyEventHandler"+c.scannerHandle;"undefined"===typeof c.barcodeDataReadyListeners[c.scannerHandle]&&(c.barcodeDataReadyListeners[c.scannerHandle]=[]);a=!1;e=c.barcodeDataReadyListeners[c.scannerHandle];for(var f=0,g=e.length;f<g;f++)if(e[f]===b){a=!0;break}a||e.push(b);window[d]=function(a){var b,e=null,f=null;if(a){try{b=JSON.parse(a)}catch(g){c.log("Failed to parse event data: "+
a)}if(b&&HoneywellBarcodeReaderUtils.hasProperty(b.params,"scannerHandle",!0))if(b.params.scannerHandle===c.scannerHandle){if(HoneywellBarcodeReaderUtils.hasProperty(b.params,"data",!0)&&(HoneywellBarcodeReaderUtils.hasProperty(b.params,"symbology",!0)&&(e=b.params.symbology),HoneywellBarcodeReaderUtils.hasProperty(b.params,"timestamp",!0)&&(f=b.params.timestamp),c.barcodeDataReadyListeners[c.scannerHandle]instanceof Array)){a=c.barcodeDataReadyListeners[c.scannerHandle];for(var n=0,q=a.length;n<
q;n++)a[n](b.params.data,e,f)}}else c.log(d+" receives Unexpected scanner handle: "+b.params.scannerHandle)}}}!1===c.barcodeDataReadyHandlersRegistered[c.scannerHandle]&&(win10ScannerBridge.addEventListener("barcodedataready",window[d]),c.barcodeDataReadyHandlersRegistered[c.scannerHandle]=!0)},clearBuffer:function(){this.batchSetBuffer.length=0},close:function(a){var b=this,e;this.scannerHandle?(e=HoneywellBarcodeReaderUtils.getRandomInt(1E4,99999),win10ScannerBridge.closeAsync(e,this.scannerHandle).then(function(c){if(c){b.logVar("win10ScannerBridge.closeAsync response",
c,!1);var e;try{e=JSON.parse(c)}catch(f){HoneywellBarcodeReaderUtils.isFunction(a)&&setTimeout(function(){a({status:HoneywellBarcodeReaderErrors.JSON_PARSE_ERROR,message:"JSON-RPC parsing error in response."})},0);return}HoneywellBarcodeReaderUtils.hasProperty(e,"result",!1)?(!0===b.barcodeDataReadyHandlersRegistered[b.scannerHandle]&&(win10ScannerBridge.removeEventListener("barcodedataready",window["dataReadyEventHandler"+b.scannerHandle]),b.barcodeDataReadyHandlersRegistered[b.scannerHandle]=!1),
HoneywellBarcodeReaderUtils.isFunction(a)&&setTimeout(function(){a({status:0,message:b.MSG_OPERATION_COMPLETED})},0)):HoneywellBarcodeReaderUtils.hasJsonRpcError(e)?HoneywellBarcodeReaderUtils.isFunction(a)&&setTimeout(function(){a({status:e.error.code,message:e.error.message})},0):HoneywellBarcodeReaderUtils.isFunction(a)&&setTimeout(function(){a({status:HoneywellBarcodeReaderErrors.JSON_PARSE_ERROR,message:"JSON-RPC parsing error in response."})},0)}else HoneywellBarcodeReaderUtils.isFunction(a)&&
setTimeout(function(){a({status:HoneywellBarcodeReaderErrors.FUNCTION_FAILED,message:"Null or empty response."})},0)})):setTimeout(function(){a({status:0,message:this.MSG_READER_CLOSED})},0)},commitBuffer:function(a){function b(a,b){for(var c,d=0<=e.scannerInterface.toUpperCase().indexOf("USB")?"USB":"Internal",d='\x3c?xml version\x3d"1.0"?\x3e\r\n\x3cConfigDoc name\x3d"Data Collection Profiles"\x3e\r\n'+('  \x3cSection name\x3d"'+b+'"\x3e\r\n')+('    \x3cKey cmd\x3d"DEVICE" desc\x3d"Specifies the scanner type" list\x3d"Internal,USB" name\x3d"Device Type"\x3e'+
d+"\x3c/Key\x3e\r\n"),d=d+'    \x3cKey cmd\x3d"TYPE"\x3eIncremental\x3c/Key\x3e\r\n',d=d+'    \x3cKey cmd\x3d"APPLY"\x3etrue\x3c/Key\x3e\r\n',f=0,g=a.length;f<g;f++){var h=a[f],d=d+('    \x3cKey cmd\x3d"'+h.command+'"');if(HoneywellBarcodeReaderUtils.hasProperty(h,"valueType",!0))if("map"===h.valueType){if(HoneywellBarcodeReaderUtils.hasProperty(h,"valueMap",!0)&&h.valueMap instanceof Array&&0<h.valueMap.length){d+=' list\x3d"';c=!0;for(var k=0,l=h.valueMap.length;k<l;k++)for(var m in h.valueMap[k])c?
(d+=h.valueMap[k][m],c=!1):d+=","+h.valueMap[k][m];d+='"'}}else if("int"===h.valueType){if(HoneywellBarcodeReaderUtils.hasProperty(h,"valueRange",!0)&&h.valueRange instanceof Array&&0<h.valueRange.length)for(k=0,c=h.valueRange.length;k<c;k++)for(m in h.valueRange[k])d+=" "+m+'\x3d"'+h.valueRange[k][m]+'"'}else if("list"===h.valueType&&HoneywellBarcodeReaderUtils.hasProperty(h,"values",!0)&&h.values instanceof Array&&0<h.values.length){d+=' list\x3d"';c=!0;k=0;for(l=h.values.length;k<l;k++)c?(d+=h.values[k],
c=!1):d+=","+h.values[k];d+='"'}d+="\x3e"+h.value+"\x3c/Key\x3e\r\n"}d+="  \x3c/Section\x3e\r\n\x3c/ConfigDoc\x3e\r\n";e.logVar("EXM string",d,!1);return d}var e=this,c=[],d=[],f;if(null===this.scannerHandle)HoneywellBarcodeReaderUtils.isFunction(a)&&(f={},f.status=HoneywellBarcodeReaderErrors.NO_CONNECTION,f.message="No scanner connection",f.method=null,f.family=null,f.key=null,f.option=null,d.push(f),setTimeout(function(){a(d)},0));else if(0===e.batchSetBuffer.length)HoneywellBarcodeReaderUtils.isFunction(a)&&
(f={},f.status=HoneywellBarcodeReaderErrors.EMPTY_COMMIT_BUFFER,f.message="The commit buffer is empty, nothing to commit.",f.method=null,f.family=null,f.key=null,f.option=null,d.push(f),setTimeout(function(){a(d)},0));else{for(var g=0,p=e.batchSetBuffer.length;g<p;g++){var l=e.batchSetBuffer[g];0===l.status?c.push(l):(f={method:"setBuffered"},f.family=l.family,f.key=l.key,f.option=l.option,f.status=l.status,f.message=l.message,d.push(f))}0<d.length?HoneywellBarcodeReaderUtils.isFunction(a)&&setTimeout(function(){a(d)},
0):(f=b(c,"WebSDKConfig"),c=HoneywellBarcodeReaderUtils.getRandomInt(1E4,99999),win10ScannerBridge.writeExmFileAsync(c,e.scannerHandle,f,"HoneywellDecoderSettingsV2.exm","WebSDKConfig").then(function(c){var b={method:"setBuffered",family:null,key:null,option:null};if(c){e.logVar("scanner.writeExmFile response",c,!1);var f;try{f=JSON.parse(c)}catch(g){HoneywellBarcodeReaderUtils.isFunction(a)&&(b.status=HoneywellBarcodeReaderErrors.JSON_PARSE_ERROR,b.message="JSON-RPC parsing error in response.",d.push(b),
setTimeout(function(){a(d)},0));return}HoneywellBarcodeReaderUtils.hasProperty(f,"result",!1)?(b.status=0,b.message=e.MSG_OPERATION_COMPLETED):HoneywellBarcodeReaderUtils.hasJsonRpcError(f)?(b.status=f.error.code,b.message=f.error.message):(b.status=HoneywellBarcodeReaderErrors.JSON_PARSE_ERROR,b.message="JSON-RPC parsing error in response.")}else b.status=HoneywellBarcodeReaderErrors.FUNCTION_FAILED,b.message="Null or empty response.";HoneywellBarcodeReaderUtils.isFunction(a)&&(d.push(b),setTimeout(function(){a(d)},
0))}))}},enableTrigger:function(a,b){var e=this,c;this.verifyActiveConnection(b)&&HoneywellBarcodeReaderUtils.stdParamCheck(a,"enabled","boolean",b)&&(c=HoneywellBarcodeReaderUtils.getRandomInt(1E4,99999),win10ScannerBridge.enableHardwareTriggerAsync(c,this.scannerHandle,a).then(function(a){if(a){e.logVar("win10ScannerBridge.enableHardwareTriggerAsync response",a,!1);var c;try{c=JSON.parse(a)}catch(g){HoneywellBarcodeReaderUtils.isFunction(b)&&setTimeout(function(){b({status:HoneywellBarcodeReaderErrors.JSON_PARSE_ERROR,
message:"JSON-RPC parsing error in response."})},0);return}HoneywellBarcodeReaderUtils.stdErrorCheck(c,b)}else HoneywellBarcodeReaderUtils.isFunction(b)&&setTimeout(function(){b({status:HoneywellBarcodeReaderErrors.FUNCTION_FAILED,message:"Null or empty response."})},0)}))},log:function(a){typeof HoneywellBarcodeReaderUtils.log===typeof Function&&HoneywellBarcodeReaderUtils.log(a)},logVar:function(a,b,e){if(typeof HoneywellBarcodeReaderUtils.log===typeof Function){var c=typeof b;"object"===c?null!==
b?"[object Array]"===Object.prototype.toString.call(b)?(a=a+"\x3d"+b.toString(),e&&(a+=", type\x3dArray")):(a=a+"\x3d"+JSON.stringify(b,null," "),e&&(a+=", type\x3dobject")):(a+="\x3dnull",e&&(a+=", type\x3dobject")):"undefined"===c?(a+="\x3dundefined",e&&(a+=", type\x3dundefined")):(a=a+"\x3d"+b.toString(),e&&(a+=", type\x3d"+c));HoneywellBarcodeReaderUtils.log(a)}},removeEventListener:function(a,b){if("barcodedataready"===a&&this.scannerHandle&&this.barcodeDataReadyListeners[this.scannerHandle]instanceof
Array)for(var e=this.barcodeDataReadyListeners[this.scannerHandle],c=0,d=e.length;c<d;c++)if(e[c]===b){e.splice(c,1);break}},setBuffered:function(a,b,e,c,d){var f=0<=this.scannerInterface.toUpperCase().indexOf("USB"),g;g={};g.family=a;g.key=b;g.option=e;"undefined"===typeof HowneywellBarcodeReaderSwiftSettings||"undefined"===typeof HowneywellBarcodeReaderRingSettings?HoneywellBarcodeReaderUtils.isFunction(d)&&(g.status=HoneywellBarcodeReaderErrors.MISSING_SETTINGS_DEF,g.message="Missing settings definition HowneywellBarcodeReaderSwiftSettings or HowneywellBarcodeReaderRingSettings.",
setTimeout(function(){d(g)},0)):null===this.scannerHandle?HoneywellBarcodeReaderUtils.isFunction(d)&&(g.status=HoneywellBarcodeReaderErrors.NO_CONNECTION,g.message="No scanner connection",setTimeout(function(){d(g)},0)):HoneywellBarcodeReaderUtils.stdParamCheckResult(a,"family","string",g,d)&&(HoneywellBarcodeReaderUtils.stdParamCheckResult(b,"key","string",g,d)&&HoneywellBarcodeReaderUtils.stdParamCheckResult(e,"option","string",g,d)&&HoneywellBarcodeReaderUtils.stdParamCheckResult(c,"value","string",
g,d))&&(a=HoneywellBarcodeReaderUtils.getSettingDef(f?HowneywellBarcodeReaderRingSettings:HowneywellBarcodeReaderSwiftSettings,a,b,e,c,!0),this.logVar("setBuffered settingDef",a,!1),this.batchSetBuffer.push(a),0===a.status?HoneywellBarcodeReaderUtils.isFunction(d)&&(g.status=0,g.message="Set request successfully buffered.",setTimeout(function(){d(g)},0)):HoneywellBarcodeReaderUtils.isFunction(d)&&(g.status=a.status,g.message=a.message,setTimeout(function(){d(g)},0)))},verifyActiveConnection:function(a){return null===
this.scannerHandle?(HoneywellBarcodeReaderUtils.isFunction(a)&&setTimeout(function(){a({status:HoneywellBarcodeReaderErrors.NO_CONNECTION,message:"No scanner connection"})},0),!1):!0}};HoneywellBarcodeReadersSwiftWin=function(a){HoneywellBarcodeReaderUtils.isFunction(a)&&setTimeout(function(){a({status:0,message:HoneywellBarcodeReaderUtils.MSG_OPERATION_COMPLETED})},0)};HoneywellBarcodeReadersSwiftWin.prototype={version:"1.20.00.0825",getAvailableBarcodeReaders:function(a){function b(b,d){if(b){var e;
try{e=JSON.parse(b)}catch(g){if(d){setTimeout(function(){a([])},0);return}return[]}if(HoneywellBarcodeReaderUtils.hasProperty(e,"result",!0))if(HoneywellBarcodeReaderUtils.hasProperty(e.result,"deviceInfoList",!0))if(d)setTimeout(function(){a(e.result.deviceInfoList)},0);else return e.result.deviceInfoList;else if(d)setTimeout(function(){a([])},0);else return[];else if(d)setTimeout(function(){a([])},0);else return[]}else if(d)setTimeout(function(){a([])},0);else return[]}var e;e=HoneywellBarcodeReaderUtils.getRandomInt(1E4,
99999);if(HoneywellBarcodeReaderUtils.isFunction(a))return win10ScannerBridge.getAvailableBarcodeReadersAsync(e).then(function(a){b(a,!0)}),[];e=win10ScannerBridge.getAvailableBarcodeReaders(e);return b(e,!1)}}})();
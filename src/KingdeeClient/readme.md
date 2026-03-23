### 主要解决金蝶sdk不用心,最新版本.Net 5.0,还在用WebRequest(.Net 官方不建议的API,很可能在未来某一天,就被移除了)
#### 主要是为了兼容金蝶sdk,所以才用的net10.0,如果不考虑兼容性,完全可以用netstandard2.0,这样就可以在netcore和netframework上都能使用了
#### 使用HttpClient代替WebRuest,做成中间件,方便使用

##### 请求参数
```javascript
{
  "format": 1,
  "useragent": "ApiClient",
  "rid": "-1555922755",
  "parameters": "[\"{\\\"FormId\\\":\\\"BD_SETTLETYPE\\\",\\\"FieldKeys\\\":\\\"FNumber\\\",\\\"FilterString\\\":\\\"FName='电汇'\\\",\\\"OrderString\\\":null,\\\"TopRowCount\\\":0,\\\"StartRow\\\":0,\\\"Limit\\\":200}\"]",
  "timestamp": "2026-03-23T15:08:03.3760146+08:00",
  "v": "1.0"
}
```

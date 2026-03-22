### 主要解决金蝶sdk不用心,最新版本.Net 5.0,还在用WebRequest(.Net 官方不建议的API,很可能在未来某一天,就被移除了)
#### 主要是为了兼容金蝶sdk,所以才用的net10.0,如果不考虑兼容性,完全可以用netstandard2.0,这样就可以在netcore和netframework上都能使用了
#### 使用HttpClient代替WebRuest,做成中间件,方便使用
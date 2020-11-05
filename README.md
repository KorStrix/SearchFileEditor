# SearchFileEditor
이 프로젝트는 .net core 콘솔 프로젝트이며,

인자로 -example을 실행시키면 json, yaml 파일이 생성됩니다.

-run (config file path)를 하면 json, yaml 파일 내 변수가 일괄 변경됩니다.

Github Repository를 Template으로 새로 만들 때마다 githubaction의 .yml 파일 및 기타 파일을 수정할 일이 많아서 만들었습니다.


## 해야 할 일
yml의 경우 Comment가 없어지는 현상을 발견

파일 하나 성공적으로 변경될 때마다 Console.WriteLine 넣기

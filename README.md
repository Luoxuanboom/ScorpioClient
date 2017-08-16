# ScorpioClient

Unity 版本： 2017.1.0

playerSetting 里把 .Net 改为4.6

## Git使用

+ 只有BlackZijian可合并到master分支
+ 开发人员开发新功能时从develop分支拉出新分支，做修改
+ 每天至少一次从develop分支合并到功能分支并解决冲突
+ 开发完成后从develop分支合并到功能分支，确保无冲突和bug后合并回develop分支
+ exitraZhao只能拉代码测试，不能push
+ 解决develop分支上的bug时拉取bug_fix分支，解决后合并到develop分支

## 代码规范

+ 在每个代码文件头注释上该文件作者、最后修改人、代码功能简介、最后修改时间

+ 每个函数都要有

  /// <summary>

  ///  

  /// </summary>

+ 每个public变量都要用对应的private变量和get set包装起来（结构体和信息存储结构的类不用）

+ 不管是否需要重载，对父类的虚函数都需要override，不需修改逻辑的要执行base.function

+ 私有变量命名规则：mPrivateVarible

+ 共有变量命名：PublicVarible

+ 临时变量命名：tempVarible
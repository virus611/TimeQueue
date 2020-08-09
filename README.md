# TimeQueue
一个带时间的消息队列项目，是本人从现有的项目中抽取出来开源的。

#项目背景
其实这事要从产品的某个需求说起。在做商城时产品妹子提了一系列的订单后续功能：
1.在生成订单3小时内需要支付，否则库存还原
2.给24小时的时间，如果没有支付订单失效
3.订单支付成功15天后自动收货
4.以上要求精确到秒

如果采用传统的Timer去循环处理，理论上是没有问题的，但是查询数据和频次太高，数据库压力太大。在多方寻求现有组件，特别是.net的组件无果后，本人决定自撸了。

#原理
数据结构里的队列，没错，C语言版的用.net core写一遍就行。所有的请求都入队列（时间顺序），第三方的接口不停的调用pop接口拿数据。由于数据都在内存里而不是数据库，性能不用我再表述了吧。

#其它
1.这个项目的功能太简单，1核1G1M的服务器都能跑，kestrel过于强悍，500qps没啥压力（每秒500个已经付了钱的订单，这个商城已经做得已经很牛X了，同代码换台好点的服务器就OK）
2.建议布置在linux上，队列不会轻易的给你GC掉（IIS上就喜欢无事给你重启，可能是我不知道怎么配置程序池。所以我加了场景恢复的功能）
3.这是一个简单的webapi项目，POP端用.NET写一个exe一直跑就行，数据量大了多运行几个就行
4.kestrel性能过于强悍，只要程序不是太渣不使用docker，单体上3K的QPS没太大的压力

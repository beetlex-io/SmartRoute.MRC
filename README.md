# Message routing center(MRC)
  MRC是基于SmartRoute扩展的消息路由中心，它的主要作用是以集群的方式支持大规模的用户消息路由转发。其设计是基于网状结构的用户中心和网关互通机制来实现消息转发；由于中心和网关都可以自由扩展其部署数量，所以在不增加开发成本的情况即可实现更大用户规模的支撑。MRC阶段只实现了用户中心和网关交互的对接，对于和客户端对接则可以根据自己需求采用自有的通讯方式。
    ![image](https://github.com/IKende/SmartRoute.MRC/blob/master/mrc.jpg)

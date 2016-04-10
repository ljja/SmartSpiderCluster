# SmartSpiderCluster
网页全网采集系统，是一款基于http协议的Web信息采集软件，支持集群化部署！

# 爬虫架构

![爬虫架构](http://scrapy-chs.readthedocs.org/zh_CN/latest/_images/scrapy_architecture.png)

参考设计：http://scrapy-chs.readthedocs.org/zh_CN/latest/topics/architecture.html

# 爬虫类型
单机、集群

# 爬虫接口
CrawlerEngine:系统整体调度器

Scheduler：调度器/抓取队列

Downloader:下载器

Spiders:蜘蛛/新内容发现

Message Pipeline:消息管道/数据清洗

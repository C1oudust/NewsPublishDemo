# 简介
.NET core MVC架构的新闻网站。
基于.NET core 2.2版本


# 目录结构描述                  
├── NewsPublish                 // web端
├── NewsPublish.Service         // 服务器端
│   ├── BannerService.cs        
│   ├── CommentService.cs                         
│   ├── Db.cs                    
│   └── NewsService.cs         
├── NewsPublish.Model  			// 实体模型类库
│   ├── Entity            		
│   │ 	├── Banner.cs           
│   │ 	├── News.cs                             
│   │ 	├── NewsClassify.cs           	
│   │ 	└── NewsComment.cs  	
│   ├── Request                 // 发送请求
│   │   ├──	AddBanner.cs        
│   │ 	├── AddComment.cs                 
│   │ 	├── AddNewsClassify.cs                
│   │ 	├── EditNewsClassify.cs        		
│   │ 	└── AddNews.cs
│   └── Response     			// 接收数据
│       ├──	BannerModel.cs       
│    	├── CommentModel.cs                  
│    	├── NewsClassifyModel.cs             
│    	├── NewsModel.cs        		
│   	└── ResponseModel.cs
├── LICENSE 
└── Readme.md 

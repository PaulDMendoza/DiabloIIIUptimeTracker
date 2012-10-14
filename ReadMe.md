This is a site that monitors the status of Diablo 3 servers and how often they are up or down. This site is running at http://www.diablo3downtime.com. 

Built using ASP.NET MVC 3 and a Windows service for the polling of the site. The visualization is all done using Flotr2 JavaScript libary. This only supports browsers like IE9, Firefox and Chrome.

#Getting Started
What you need to program this:
- SQL Server 2008 Express or above
- Visual Studio 2012
- .NET 4.5
- ASP.NET MVC 3

##Running the code
There are two compontents of running the code. There is running the Windows service and running the website. The Windows service will generate the data and the site will consume the data. 

###Running the web service
Ensure the Web.Config connection string points to the correct SQL Server instance. By default I've configured the code to point at SQLExpress local. 

Next, build the service and install it. It's a basic Windows Service so you can lookup online if you don't know how to do this.

Any service failures are written to the Event log.

You shouldn't make the database. The Entity Framework model of the database will make the database for you.

###Running the site
Ensure the web.config is configured correctly.

Start the website. Most likely you're missing data so the site will be an empty page. You can use the below scripts to generate data into your database to play with instead of running the Windows Service for a month.

#Generating Test Data

## Generate Reading Categories
Run this once against your database. The unit tests actually generate this data for you in the test database.

```
INSERT [dbo].[PollCategory] ([PollCategoryID], [ServerCategory], [Region]) VALUES (N'91cf2cab-1807-487c-9c11-00038a13b4d3', N'Game Server', N'Americas')
INSERT [dbo].[PollCategory] ([PollCategoryID], [ServerCategory], [Region]) VALUES (N'58b58d29-e06d-43e3-9b23-18ebc54482c1', N'Gold', N'Americas')
INSERT [dbo].[PollCategory] ([PollCategoryID], [ServerCategory], [Region]) VALUES (N'9044db2d-4ad9-494b-98b8-1ee80a5ba830', N'Hardcore', N'Asia')
INSERT [dbo].[PollCategory] ([PollCategoryID], [ServerCategory], [Region]) VALUES (N'48836001-d45b-44c0-a0dc-2a833df9801f', N'Game Server', N'Asia')
INSERT [dbo].[PollCategory] ([PollCategoryID], [ServerCategory], [Region]) VALUES (N'97245c73-27d5-43f8-8ff6-3a585e987d7c', N'EUR', N'Europe')
INSERT [dbo].[PollCategory] ([PollCategoryID], [ServerCategory], [Region]) VALUES (N'7a7d20e0-143d-4330-9205-3f0a9fe00eb1', N'Hardcore', N'Americas')
INSERT [dbo].[PollCategory] ([PollCategoryID], [ServerCategory], [Region]) VALUES (N'f58c5e02-2b68-4f6a-a9fe-4610c68c0dd5', N'Gold', N'Europe')
INSERT [dbo].[PollCategory] ([PollCategoryID], [ServerCategory], [Region]) VALUES (N'ee5b8adf-43d4-474b-9f9b-90cedebefbfa', N'Game Server', N'Europe')
INSERT [dbo].[PollCategory] ([PollCategoryID], [ServerCategory], [Region]) VALUES (N'8295bfbd-21d1-4958-b47e-910c69b7ece7', N'USD', N'Americas')
INSERT [dbo].[PollCategory] ([PollCategoryID], [ServerCategory], [Region]) VALUES (N'ef82da16-3b0b-4e9c-92db-9590f6340050', N'Gold', N'Asia')
INSERT [dbo].[PollCategory] ([PollCategoryID], [ServerCategory], [Region]) VALUES (N'58fec68f-79f2-419f-8df3-b41a0a77dbfc', N'Hardcore', N'Europe')
```


## Generate sample reads
Run the following SQL to generate a bunch of sample website reads in your database.

```
insert into dbo.PollCategoryValue (CreatedTime, CategoryID, [Status])
select 
  DATEADD(HOUR, numbers.rn * -1, GETDATE()),
	pc.PollCategoryID,
	1
from
(SELECT number AS rn FROM master.dbo.spt_values 
WHERE type = 'P' AND number BETWEEN 0 AND 200) numbers
left join PollCategory pc on 1 = 1
```

#Profiling Performance
The code uses the Stack Exchange MVC Mini Profiler to help identify performance issues by showing a little icon at the top left. Click the icon to see a breakdown of performance.

To show the icon, add <b>"?profile=true"</b> to the end of the URL

http://localhost:55071/Home/Index?profile=true

The caching can cause weird stuff to happen though so be aware that you will see a bunch of requests if the site has been running and cached results were being returned.

#License
Copyright (C) 2012 Paul Mendoza

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


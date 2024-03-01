
using HtmxBlog.Modules;

var builder = WebApplication.CreateBuilder(args);

///********************ServicePoint*********************************************************

builder.RegisterServices();

var app = builder.Build();


///********************MiddlePoint**********************************************************

app.RegisterMiddlewares();

///********************EndPoint*************************************************************
app.RegisterStaticsEndpoints();
app.RegisterPostsHtmlEndpoints();
app.RegisterPostsEndpoints();
app.RegisterFileUploadEndpoints();

///*****************************************************************************************


app.Run();

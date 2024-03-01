
using HtmxBlog.Modules;

var builder = WebApplication.CreateBuilder(args);

/// Add services to the container.
builder.RegisterServices();

var app = builder.Build();




app.RegisterMiddlewares();

///********************Static API*************************************************************
app.RegisterStaticsEndpoints();

///********************HTML API***************************************************************

app.RegisterPostsHtmlEndpoints();

///////////////////////

///********************Post API***************************************************************

app.RegisterPostsEndpoints();

///********************File Upload API*********************************************************
app.RegisterFileUploadEndpoints();

///********************************************************************************************


app.Run();

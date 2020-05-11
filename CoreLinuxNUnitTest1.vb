Imports NUnit.Framework
Imports Moq
Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.Hosting
Imports Microsoft.Extensions.Logging
Imports Microsoft.Extensions.Logging.Abstractions
Imports CoreLinux
Imports Newtonsoft.Json
Imports Microsoft.AspNetCore.Mvc

Namespace $safeprojectname$

    Public Class Tests
        Property CoreDIserviceCollection As IServiceCollection
        Property CoreDIserviceProvider As ServiceProvider
        Property MockLog As ILogger(Of CoreLinux.TestController)
        Property RealLog As ILogger(Of CoreLinux.TestController)
        Property AuWcf1 As AuWcf

        <SetUp>
        Public Sub Setup()
            CoreDIserviceCollection = New ServiceCollection()
            CoreDIserviceProvider = CoreDIserviceCollection.AddLogging.BuildServiceProvider()
        End Sub

        <Test>
        Public Sub ShowServive()
            Dim Str1 As New Text.StringBuilder
            CoreDIserviceCollection.ToList.ForEach(Sub(ServiceDescriptor)
                                                       Str1.Append(ServiceDescriptor.Lifetime.ToString & " : ")
                                                       Str1.AppendLine(ServiceDescriptor.ServiceType.FullName)
                                                   End Sub)
            Assert.IsTrue(CoreDIserviceCollection.Count > 0)
            TestContext.WriteLine("CoreDI builded " & CoreDIserviceCollection.Count & " services" & vbCrLf & Str1.ToString)
            TestContext.WriteLine()
        End Sub

        <Test>
        Public Sub CreateMiddlwareInstance()
            AuWcf1 = New AuWcf()
            Assert.AreEqual(5, AuWcf1.WcfConfig.Count)
            TestContext.WriteLine("WcfConfig" & vbCrLf & JsonConvert.SerializeObject(AuWcf1.WcfConfig, Formatting.Indented))
            TestContext.WriteLine()
        End Sub

        <Test>
        Public Sub TestCoreSingletonServiceFactory()
            ShowServive()
            CreateMiddlwareInstance()
            CoreDIserviceCollection.AddSingleton(GetType(IAuWcf), AuWcf1)
            Dim Res As String = AuWcf1.AuTest(10)
            Assert.AreEqual("10", Res)
            ShowServive()
        End Sub

        <Test>
        Public Sub TestCoreControllerWithoutLog()
            CreateMiddlwareInstance()
            Dim Controller1 As New CoreLinux.TestController(AuWcf1)
            Dim Response As IActionResult = Controller1.Index()
            Dim Content As ContentResult = Response
            TestContext.WriteLine(Content.Content)
            Assert.AreEqual(Content.ContentType, "text/html")
        End Sub

        <Test>
        Public Sub AddMockLoggerWithOwnImpl()
            ShowServive()
            Dim _Mock = New Mock(Of ILogger(Of CoreLinux.TestController))
            MockLog = _Mock.Object
            Dim MockLogger1 As MockLogger = New MockLogger()
            MockLogger1.LogLevel = LogLevel.Information Or LogLevel.Error Or LogLevel.Warning Or LogLevel.Trace Or LogLevel.Debug Or LogLevel.Trace
            CoreDIserviceCollection.AddSingleton(Of ILoggerFactory)(Function(s)
                                                                        Dim loggerFactoryMock = New Mock(Of ILoggerFactory)()
                                                                        loggerFactoryMock.Setup(Function(m) m.CreateLogger(It.IsAny(Of String)())).Returns(MockLogger1)
                                                                        Return loggerFactoryMock.Object
                                                                    End Function)

            MockLog.Log(LogLevel.Information, "MockLog")
            ShowServive()
        End Sub

        <Test>
        Public Sub AddMockLoggerSimple()
            Dim _Mock = New Mock(Of ILogger(Of CoreLinux.TestController))
            MockLog = _Mock.Object
            CoreDIserviceCollection.AddSingleton(GetType(ILogger(Of TestController)), MockLog)
            MockLog.Log(LogLevel.Information, "MockLog")
            ShowServive()
        End Sub

        <Test>
        Public Sub TestCoreControllerWithMockLog()
            AddMockLoggerSimple()
            CreateMiddlwareInstance()
            Dim Controller1 As New CoreLinux.TestController(AuWcf1, MockLog)
            Dim Response As IActionResult = Controller1.Index()
            Dim Content As ContentResult = Response
            TestContext.WriteLine(Content.Content)
            Assert.AreEqual(Content.ContentType, "text/html")
        End Sub

        <Test>
        Public Sub AddRealLogger()
            CreateMiddlwareInstance()
            Dim _LoggerFactory = LoggerFactory.Create(Sub(Builder) Builder.AddConsole())
            RealLog = _LoggerFactory.CreateLogger(Of CoreLinux.TestController)
            CoreDIserviceCollection.AddSingleton(GetType(ILogger(Of TestController)), RealLog)
            RealLog.LogInformation("Test RealLog")
            ShowServive()
        End Sub

        <Test>
        Public Sub TestCoreControllerWithRealLog()
            CreateMiddlwareInstance()
            AddRealLogger()
            Dim Controller1 As New CoreLinux.TestController(AuWcf1, RealLog)
            Dim Response As IActionResult = Controller1.Index()
            Dim Content As ContentResult = Response
            TestContext.WriteLine(Content.Content)
            Assert.AreEqual(Content.ContentType, "text/html")
        End Sub

    End Class

End Namespace
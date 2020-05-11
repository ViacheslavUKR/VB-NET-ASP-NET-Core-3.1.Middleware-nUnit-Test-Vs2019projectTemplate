Imports System
Imports Microsoft.Extensions.Logging
Imports NUnit.Framework
Imports Moq

Class MockLogger
    Implements ILogger

    ''' <summary>
    ''' Last LogLevel called.
    ''' </summary>
    Public Property LogLevel As LogLevel?

    ''' <summary>
    ''' Last Event Id.
    ''' </summary>
    Public Property EventId As EventId?

    ''' <summary>
    ''' Last Exception passed in.
    ''' </summary>
    Public Property Exception As Exception

    ''' <summary>
    ''' Last Message after running through the formatter.
    ''' </summary>
    Public Property Message As String

    Public Sub New()
        Reset()
    End Sub

    ''' <summary>
    ''' Reset captured log data.
    ''' </summary>
    Public Sub Reset()
        LogLevel = Nothing
        EventId = Nothing
        Exception = Nothing
        Message = Nothing
    End Sub

    ''' <summary>
    ''' Assert that the values are as expected.
    ''' </summary>
    ''' <param name="logLevel">Log Level</param>
    ''' <param name="eventId">Event ID</param>
    ''' <param name="exception">Exception</param>
    ''' <param name="message">Formatted Message</param>
    Public Sub Assert(ByVal logLevel As LogLevel?, ByVal eventId As EventId?, ByVal exception As Exception, ByVal message As String)
        NUnit.Framework.Assert.AreEqual(logLevel, logLevel)
        NUnit.Framework.Assert.AreEqual(eventId, eventId)
        NUnit.Framework.Assert.AreEqual(exception, exception)
        NUnit.Framework.Assert.AreEqual(message, message)
        NUnit.Framework.Assert.[True](IsEnabled(Microsoft.Extensions.Logging.LogLevel.Debug))

        Using scope = BeginScope("Test")
            NUnit.Framework.Assert.IsAssignableFrom(Of IDisposable)(scope)
        End Using
    End Sub

    ''' <summary>Writes a log entry.</summary>
    ''' <param name="logLevel">Entry will be written on this level.</param>
    ''' <param name="eventId">Id of the event.</param>
    ''' <param name="state">The entry to be written. Can be also an object.</param>
    ''' <param name="exception">The exception related to this entry.</param>
    ''' <param name="formatter">Function to create a <c>string</c> message of the <paramref name="state" /> And <paramref name="exception" />.</param>
    Public Sub Log(Of TState)(logLevel As LogLevel, eventId As EventId, state As TState, exception As Exception, formatter As Func(Of TState, Exception, String)) Implements ILogger.Log
        logLevel = logLevel
        eventId = eventId
        exception = exception
        Message = formatter(state, exception)
    End Sub

    ''' <summary>Begins a logical operation scope.</summary>
    ''' <param name="state">The identifier for the scope.</param>
    ''' <returns>An IDisposable that ends the logical operation scope on dispose.</returns>
    Public Function BeginScope(Of TState)(state As TState) As IDisposable Implements ILogger.BeginScope
        Return NullDisposable.Instance
    End Function

    ''' <summary>
    ''' Checks if the given <paramref name="logLevel" /> Is enabled.
    ''' </summary>
    ''' <param name="logLevel">level to be checked.</param>
    ''' <returns><c>true</c> if enabled.</returns>
    Public Function IsEnabled(logLevel As LogLevel) As Boolean Implements ILogger.IsEnabled
        Return True
    End Function

    Private Class NullDisposable
        Implements IDisposable

        Friend Shared ReadOnly Instance As IDisposable = New NullDisposable()

        Public Sub Dispose() Implements IDisposable.Dispose
        End Sub
    End Class
End Class

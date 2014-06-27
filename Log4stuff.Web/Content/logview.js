/*global $, angular, console */
(function() {
    'use strict';

    var app = angular.module('logView', ['ngRoute']);

    app.controller('LogController', function($scope, $window) {
        $scope.logMessages = [];
        $scope.maxMessages = 1000;

        $scope.subscribe = function() {
            if (!$window.applicationId) {
                console.warn("Can't subscribe to an empty applicationId");
                return;
            }
            
            $("#logTable th").resizable(
                {
                    handles: "e"
                }
            );

            $scope.applicationId = $window.applicationId;

            var logHub = $.connection.logMessageHub;

            //Register the application id we're interested in
            $.connection.hub.qs = { 'applicationId': $scope.applicationId };

            logHub.client.newLogMessage = function(logMessage) {
                var log = JSON.parse(logMessage),
                    hostName = null;
                
                if (log.Metadata && log.Metadata) {
                    hostName = log.Metadata['log4net:HostName'];
                }
                
                $scope.logMessages.push({
                    timestamp: log.Timestamp,
                    level: log.Level,
                    logger: log.Logger,
                    thread: log.Thread,
                    message: log.Message,
                    hostName: hostName
                });

                if ($scope.logMessages.length > $scope.maxMessages) {
                    $scope.logMessages.shift();
                }

                $scope.$apply();
            };

            $.connection.hub.start().done(function() {
                //var logMessageHub = $.connection.logMessageHub;
                //logMessageHub.client.registerClient('appid1');
                console.log('Connected to the SignalR hub');
            }).fail(function() {
                console.error('Could not connect to the SignalR hub :-(');
            });
        };

        $scope.subscribe();
    });
})();
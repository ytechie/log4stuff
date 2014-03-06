/*global $, angular, console */
(function() {
    'use strict';

    var app = angular.module('logView', ['ngRoute']);

    app.controller('LogController', function($scope, $window) {
        $scope.logMessages = [];
        $scope.maxMessages = 1000;

        $scope.subscribe = function() {
            if (!$scope.applicationId) {
                console.warn("Can't subscribe to an empty applicationId");
                return;
            }

            var logHub = $.connection.logMessageHub;

            //Register the application id we're interested in
            $.connection.hub.qs = { 'applicationId': $scope.applicationId };


            logHub.client.newLogMessage = function(logMessage) {
                var log = JSON.parse(logMessage).event;
                var hostName;

                for (var i = 0; i < log.properties.data.length; i++) {
                    if (log.properties.data[i]['@name'] == 'log4net:HostName') {
                        hostName = log.properties.data[i]['@value'];
                    }
                }
                $scope.logMessages.push({
                    timestamp: new Date(),
                    level: log['@level'],
                    logger: log['@logger'],
                    thread: log['@thread'],
                    message: log.message,
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

        if ($window.applicationId) {
            $scope.applicationId = $window.applicationId;
            $scope.subscribe();
        }
    });
})();
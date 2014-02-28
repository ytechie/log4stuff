/*global $, angular, console */
(function(){
    'use strict';

    var app = angular.module('logView', []);

    app.controller('LogController', function($scope) {
        $scope.logMessages = [];

        $scope.subscribe = function () {
            if (!$scope.applicationId) {
                console.warn("Can't subscribe to an empty applicationId");
                return;
            }

            var logHub = $.connection.logMessageHub;

            //Register the application id we're interested in
            $.connection.hub.qs = { 'applicationId': $scope.applicationId };


            logHub.client.newLogMessage = function(logMessage) {
                var log = JSON.parse(logMessage).event;

                $scope.logMessages.push({
                    timestamp: new Date(),
                    level: log['@level'],
                    logger: log['@logger'],
                    thread: log['@thread'],
                    message: log.message,
                    hostName: log.properties.data[0]['@value']
                });
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
    });
})();
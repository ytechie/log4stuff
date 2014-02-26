/*global $, angular, console */
(function(){
    'use strict';

    var app = angular.module('logView', []);

    app.controller('LogController', function($scope) {
        $scope.logMessages = [];

        var logHub = $.connection.logMessageHub;
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
    
        $.connection.hub.start();
        console.log('SignalR Hub Connection Opened');
    });
})();
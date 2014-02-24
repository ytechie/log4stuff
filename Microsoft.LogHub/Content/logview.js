var logHub = $.connection.logMessageHub;
var log;

function ticksToTimestamp(ticks) {
    var d = new Date();
    d.setTime(ticks);
    return d;
}

logHub.client.newLogMessage = function (logMessage) {
    log = JSON.parse(logMessage).event;
    $("#logTable tr:last").after(
        '<tr>' +
        '<td>' + ticksToTimestamp(log['@timestamp']) + '</td>' +
        '<td>' + log['@level'] + '</td>' +
        '<td>' + log['@logger'] + '</td>' +
        '<td>' + log['@thread'] + '</td>' +
        '<td>' + log.message + '</td>' +
        '<td>' + log.properties.data[0]['@value'] + '</td>' +
        '</tr>');
};

$.connection.hub.start();
$(function () {
    var conn = 'http://localhost/Scheduler/mysignalr';
    var connection = $.hubConnection(conn, { useDefaultPath: false });
    var newHub = connection.createHubProxy('taskHub2');

    var hub = $.connection.taskHub;

    $.connection.hub.start().done(function () {
    });

    hub.client.deleteTask = function (id) {
        $('#' + id).parent().parent().parent().remove();
    };

    hub.client.endDate = function (endDate, id) {
        $('#' + id).parent().parent().next().next().next().text(endDate);
    };

    hub.client.disable = function (name, isDisabled, taskId) {

        if (name == 'cancel') {
            $('#' + taskId).parent().parent().siblings().last().children().attr('disabled', isDisabled);
        }
        else {
            $('#' + taskId).parent().parent().siblings().last().prev().children().attr('disabled', isDisabled);
        }

    };

    hub.client.addTask = function (jsonTask) {
        var task = JSON.parse(jsonTask);

        $('#tbody' + task.Type).append(
            `<tr>
            <td>
                <div class="progress">
                    <div class="progress-bar progress-bar-success" role="progressbar"
                        aria-valuenow="${task.Percentage}"
                        aria-valuemin="0" aria-valuemax="100" style="width:${task.Percentage}%" id="${task.Id}">
                    </div>
                </div>
            </td>
            <td>Created</td>
            <td>${task.StartDate}</td>
            <td>${task.EndDate}</td>
            <td><a id="a-${task.Id}"></a></td>
            <td><button class="btn btn-default delBtn" disabled>Delete</button></td>
            <td><button id="" class="btn btn-default cancelBtn">Cancel</button></td>
        </tr>`);
        
        newHub.invoke('startTask', task.Type, $('#userId').text(), task.Id);
    };


    connection.start().done(function () {
        newHub.invoke('connect', $('#userId').text());

        $('#containerId').on('click', '.addTaskButton', function () {
            var idBtn = $(this).attr('id');
            if (idBtn=='btnid1')
                hub.invoke('addTask', $('#userId').text(), 1);
            else
                hub.invoke('addTask', $('#userId').text(), 2);
        });

        $('.tbody').on('click', '.delBtn', function () {
            hub.server.deleteTask($(this).parent().siblings().first().children().children().attr('id'));
        });

        $('.tbody').on('click','.cancelBtn', function () {
            var btn = $(this);
            var taskId = btn.parent().siblings().first().children().children().attr('id');
            var idBtn = btn.parent().parent().parent().attr('id');
            if (idBtn == 'tbody1')
                newHub.invoke('cancel', taskId, 1);
            else
                newHub.invoke('cancel', taskId, 2);
        });
    });


    newHub.on('setResult', function (taskId, res) {

        var news =  JSON.parse(res);

        if (news.length == 2) {
            hub.server.setResult(taskId, news[0].url, news[0].title);
            $('#a-' + taskId).attr('href', news[0].url);
            $('#a-' + taskId).text(news[0].title);

        }
        else {
            hub.server.setResult(taskId, news[0][0].url, news[0][0].title);
            $('#a-' + taskId).attr('href', news[0][0].url);
            $('#a-' + taskId).text(news[0][0].title);
        }
    });

    newHub.on('cancel', function (id) {
        $('#' + id).parent().parent().next().text('Canceled');
        $('#' + id).attr('class', 'progress-bar progress-bar-danger');
        hub.server.cancel(id);
    });

    newHub.on('increase', function (id, num, state) {
        hub.server.updateTask(id, num);

        if (state !== "") {
            $('#' + id).parent().parent().next().text(state);
        }

        $('#' + id).attr("aria-valuenow", num);
        $('#' + id).attr("style", "width:" + num + "%");
    });
    
});
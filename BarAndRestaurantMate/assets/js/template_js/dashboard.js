var calendar, d, date, m, y;

date = new Date();

d = date.getDate();

m = date.getMonth();

y = date.getFullYear();

calendar = $("#calendar").fullCalendar({
    header: {
        left: "prev,next today",
        center: "title",
        right: "month,agendaWeek,agendaDay"
    },
    selectable: true,
    selectHelper: true,
    select: function (start, end, allDay) {
        var title;
        title = prompt("Event Title:");
        if (title) {
            calendar.fullCalendar("renderEvent", {
                title: title,
                start: start,
                end: end,
                allDay: allDay
            }, true);
        }
        return calendar.fullCalendar("unselect");
    },
    editable: true,
    events: "/Calendar/GetMyEvents"
    //    [
    //  {
    //    title: "School Holidays",
    //    start: new Date(y, m, 3, 12, 0),
    //    end: new Date(y, m, 7, 14, 0),
    //  }, {
    //    title: "Maths Class",
    //    start: new Date(y, m, d, 12, 0),
    //    end: new Date(y, m, d+2, 14, 0),
    //    allDay: false
    //  }, {
    //    title: "Reminder for Geography Test",
    //    start: new Date(y, m, 28),
    //    end: new Date(y, m, 29),
    //    url: "http://google.com/"
    //  }
    //]
});

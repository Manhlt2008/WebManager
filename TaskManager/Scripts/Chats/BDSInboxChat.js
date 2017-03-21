
BDSInboxChat = new function () {

    var timeType =
    {
        Year: "year",
        Month: "month",
        Day: "day",
        Hour: "hour",
        Minute: "minute",
        Second: "second"
    };

    var emoticonOpt = {
        handle: '#etoggle',
        dir: '/images/emoticons/',
        label: 'On Emoticons',
        style: 'background: #eee',
        css: 'class2'
    };

    this.initInbox = function () {
        // load room
        $.ajax({
            url: '/handlers/ChatHandler.ashx',
            data: { action: "getinboxroom", userid: senderId },
            success: function (data) {
                if (data.Response == 0) {
                    for (var i = 0; i < data.Data.length; i++) {
                        var group;
                        if (data.Data[i].Product == null) {
                            group = $("#chat-user-inbox-pgroup1-Template").tmpl();
                        } else {
                            group = $("#chat-user-inbox-pgroup-Template").tmpl(data.Data[i].Product);
                        }
                        group.appendTo($(".chat-user-mgr-inbox-content").children());

                        for (var j = 0; j < data.Data[i].Rooms.length; j++) {
                            if (data.Data[i].Rooms[j].ChatMessage != null) {
                                var e = $("#chat-user-inbox-rinfo-Template").tmpl({
                                    Email: (data.Data[i].Rooms[j].Room.ClientFromUserId == senderId ? data.Data[i].Rooms[j].Room.ToFullName : data.Data[i].Rooms[j].Room.FromFullName),
                                    Message: data.Data[i].Rooms[j].ChatMessage.Message,
                                    Time: BDSInboxChat.convertTime(data.Data[i].Rooms[j].ChatMessage.CreateDateStamp, (new Date()).getTime()),
                                    ClassInbox: data.Data[i].Rooms[j].ChatMessage.FromUserId == senderId ? "outbox" : "",
                                    RoomId: data.Data[i].Rooms[j].Room.ChatRoomId,
                                    ProductId: data.Data[i].Rooms[j].Room.ProductId,
                                    UserId: data.Data[i].Rooms[j].Room.ClientFromUserId == senderId ? data.Data[i].Rooms[j].Room.ClientToUserId : data.Data[i].Rooms[j].Room.ClientFromUserId,
                                });
                                e.appendTo(group.find("ul"));
                                e.click(function() {
                                    var roomId = $(this).attr("roomId");
                                    if ($("#chatPopup-" + roomId).length == 0) {
                                        var userId = $(this).attr("userId");
                                        var productId = $(this).attr("productId");
                                        BDSChat.initiateChat(userId, productId);
                                    }
                                });
                                e.emoticons(emoticonOpt);
                            }
                        }
                    }
                }
            }
        });
    };

    this.addChatArea = function (chatRoomId, title, fullName, chatArea) {
        // Xóa room cũ
        this.closeRoom();

        var divChatPopup = $("#chat-inbox-popup-template").tmpl({ id: chatRoomId, fullName: fullName }).appendTo($('.chat-user-mgr-chat-area'));
        $("#chat-inbox-popup-header-template").tmpl().appendTo(divChatPopup);
        chatArea.appendTo(divChatPopup);
        divChatPopup.find(".chatPopup-header .header-title").append(title);

        // Hard style
        divChatPopup.find(".chatPopup-header").css({
            "background-color": "#ededed",
            "border-bottom": "1px solid #999999",
            "color":"#000",
            "border-radius": 0,
            "-webkit-border-radius": 0,
            "-moz-border-radius": 0,
        });

        divChatPopup.find(".user-status").removeAttr("style");

        divChatPopup.find(".link-chat-bds-header").css({
            "display": "none",
        });

        divChatPopup.find(".chat-header-bds").css({
            "background-color": "#ededed",
            "margin-left": 0
        });

        divChatPopup.find(".chatMessages .msg-text-item .bounds").css({
            "max-width" : "342px"
        });

        divChatPopup.find(".chat-command").css({
            "height" : "58px"
        });

        divChatPopup.find(".div-emoticon").css({
            "top": "7px"
        });

        divChatPopup.find(".chatNewMessage").css({
            "width": "375px",
            "min-height": "54px"
        });

        $(".chat-hiddendiv").css({
            "width": "375px",
            "min-height" :"54px"
        });
    }

    this.closeRoom = function () {
        var room = $(".chat-user-mgr .chat-user-mgr-chat-area").children();
        if (room.length > 0) {
            BDSChat.closeRoom(room.attr("roomid"));
            room.remove();
        }
    }

    this.updateStatus = function (onlineUsers) {

        var inboxRooms = $(".chat-user-mgr .chat-inbox-room");
        var isOnline = false;
        if (inboxRooms.length > 0) {
            for (var j = 0; j < inboxRooms.length; j++) {
                isOnline = false;
                for (var i = 0; i < onlineUsers.length; i++) {
                    if ($(inboxRooms[j]).attr("userid") == onlineUsers[i].UserId) {
                        isOnline = true;
                        break;
                    }
                }

                if (isOnline) {
                    $(inboxRooms[j]).find(".chat-inbox-room-info").addClass("online-status");
                } else {
                    $(inboxRooms[j]).find(".chat-inbox-room-info").removeClass("online-status");
                }
            }
        }
    };

    this.updateLastestMessage = function(chatMessage) {

    };

    this.updateUnreadMessage = function() {

    };

    this.addRoomToInbox = function(chatRoom) {

    };

    this.convertTime = function(time, currentTime) {
        var datetime = new Date(time);
        var currentDatetime = new Date(currentTime);
        
        var year = datetime.getFullYear();
        //var month = datetime.getMonth() < 10 ? ("0" + datetime.getMonth()) : datetime.getMonth();
        var month = datetime.getMonth() + 1;
        var day = datetime.getDate() < 10 ? ("0" + datetime.getDate()) : datetime.getDate();
        //var hour = datetime.getHours() < 10 ? ("0" + datetime.getHours()) : datetime.getHours();
        var hourNumber = datetime.getHours();
        var hour = hourNumber % 12 == 0 && hourNumber > 0 && hourNumber != 24 ? 12 : hourNumber % 12;
        var minute = datetime.getMinutes() < 10 ? ("0" + datetime.getMinutes()) : datetime.getMinutes();
        var second = datetime.getSeconds() < 10 ? ("0" + datetime.getSeconds()) : datetime.getSeconds();
        var amOrPm = hourNumber > 12 ? " PM" : " AM";

        if (datetime.getFullYear() < currentDatetime.getFullYear()) {
            return hour + ":" + minute + amOrPm + " " + day + " tháng " + month + " năm " + year;
        } else if (datetime.getMonth() < currentDatetime.getMonth()) {
            return hour + ":" + minute + amOrPm + " " + day + " tháng " + month;
        } else if (datetime.getDate() < currentDatetime.getDate()) {
            return hour + ":" + minute + amOrPm + " " + day + " tháng " + month;
        }
        else {
            return hour + ":" + minute + amOrPm;
        }
    }

};
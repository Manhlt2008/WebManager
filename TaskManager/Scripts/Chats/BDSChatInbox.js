var chatServerUrl = "";
var senderId = '';
var senderName = '';
var productId = 0;
var chatPK = '';

BDSChatInbox = new function () {
    var chatRooms = 0;
    //var numRand = logintUserId != 0 ? logintUserId : Math.floor(Math.random() * 1000);
    //var senderId = numRand;
    //var senderName = logintUserId != 0 ? loginFullName: ('User ' + numRand);
    //var senderId = logintUserId;
    //var senderName = loginFullName;
    
    var rooms = [];
    var timeType =
    {
        Year: "year",
        Month: "month",
        Day: "day",
        Hour: "hour",
        Minute: "minute",
        Second: "second"
    };

    var handlerUrl = "http://chat.bds.vn/handlers/ChatHandler.ashx";
    var cookieRoomName = "CHAT_ROOM";
    var roomHistories = null;
    var classTimeChat = "chat-time-";
    var defaultAvatar = "/images/no-avatar.png";
    var defaultSupportAvatar = "/images/support-avatar.png";
    var chatSoundUrl = "/Scripts/ChatSound.mp3";
    var offsetTime = 60; // 1h * 60
    
    var chatServer;
    var emoticonOpt = {
        handle: '#etoggle',
        dir: '/images/emoticons/',
        label: 'On Emoticons',
        style: 'background: #eee',
        css: 'class2'
    };

    window.onbeforeunload = function () {
        if (chatRooms > 0) {
            //return "All chat instances will be ended!";
            // Close room before unload tab/broswer
            BDSChatInbox.closeAllRoom();
        }
    };

    this.attachEvents = function () {
        //BDSChatInbox.addChatArea();
        var $this = this;
        //$("#userNameLabel").html(senderName);
        if ($.connection != null) {
            $.connection.hub.url = chatServerUrl + '/signalr/hubs';
            //$.connection.hub.qs = "__ui=" + encodeURIComponent(senderId) + "&__un=" + encodeURIComponent(senderName);
            $.connection.hub.qs = "__ui=" + encodeURIComponent(senderId) + "&__un=" + encodeURIComponent(senderName);
            chatServer = $.connection.chatServer;
            $.connection.hub.start({ transport: 'auto' }, function () {
                //BDSChatInbox.loadRoomFromCookie();
                //chatServer.server.connect(senderId, senderName).fail(function (e) {
                //    alert(e);
                //});
            }).fail(function () {
                alert("error connect");
            }).done(function () {
                //alert("connect");
                BDSChatInbox.requestInboxRooms();
            });;

            chatServer.client.initiateChatUI = function (chatRoom, isOnline, guestInfo) {
                var chatRoomDiv = $('#chatRoom-' + chatRoom.chatRoomId);
                //if (($(chatRoomDiv).length > 0)) {
                if (rooms[chatRoom.chatRoomId] != undefined) {
                    
                }
                else {
                    var toUserId = chatRoom.toUserId != senderId ? chatRoom.toUserId : chatRoom.fromUserId;
                    rooms[chatRoom.chatRoomId] = {
                        popup: null,
                        oldTime: null,
                        lastTime: null,
                        toUserId: toUserId,
                        avatarUrl: null,
                        fileId: null,
                        productId: chatRoom.productId,
                        isLoadHistory: true
                };

                    var e = $('#new-chatroom-template').tmpl(chatRoom);
                    var c = $('#new-chat-header').tmpl(chatRoom);

                    var divChatMessage = e.find('.chatMessages');

                    chatRooms++;

                    
                    // Product details
                    if (chatRoom.productId > 0) {
                        BDSChatInbox.loadProductDetail(divChatMessage, chatRoom);
                        divChatMessage.css("height", 213);
                    } else {
                        if (BDSChatInbox.loadSupportDetail(divChatMessage, chatRoom)) {

                            // fix popup support bds
                            c.find(".user-status").css("display", "none");
                            c.find(".user-status").after($("<span />").addClass("link-chat-bds-header"));
                            c.addClass("chat-header-bds");
                            divChatMessage.css("height", 213);
                        } else {
                            divChatMessage.css("height", 316);
                        }

                    }

                    
                    BDSChatInbox.addChatArea(chatRoom.chatRoomId, c, BDSChatInbox.getFullName(chatRoom.chatUsers), e);

                    // Add online User
                    if (isOnline) {
                        c.find(".user-status").addClass("online-status");
                        c.find(".link-chat-bds-header").removeClass("offline-status");
                    } else {
                        c.find(".user-status").removeClass("online-status");
                        c.find(".link-chat-bds-header").addClass("offline-status");
                    }
                    
                    // Get history
                    rooms[chatRoom.chatRoomId].isLoadHistory = false;
                    BDSChatInbox.requestHistory(chatRoom.chatRoomId);


                    $('#newmessage-' + chatRoom.chatRoomId).focus(function () {
                        if (!$(this).hasClass("text-focus")) {
                            $(this).addClass("text-focus");
                        }
                    });

                    $('#newmessage-' + chatRoom.chatRoomId).blur(function () {
                        if ($(this).hasClass("text-focus")) {
                            $(this).removeClass("text-focus");
                        }
                    });

                    $('#newmessage-' + chatRoom.chatRoomId).focus();

                    $("#messages-" + chatRoom.chatRoomId).scroll(function () {
                        if ($(this).scrollTop() == 0) {
                            var roomId = parseInt($(this).attr("id").split("-")[1]);
                            if (rooms[roomId].isLoadHistory) {
                                BDSChatInbox.requestHistory(roomId);
                            }
                        }
                    });

                    $('#sendmessage-' + chatRoom.chatRoomId).keypress(function (e) {
                        if ((e.which && e.which == 13) || (e.keyCode && e.keyCode == 13)) {
                            $('#chatsend-' + chatRoom.chatRoomId).click();
                            return false;
                        }
                    });
                    // Emoticons
                    $('#pnlEmoticions-' + chatRoom.chatRoomId).click(function (e) {
                        var pos = $(this).offset();
                        var width = $(this).outerWidth();
                        var height = $(this).outerHeight();
                        //var postLeft = Math.floor(($('#emoticons').outerWidth() - $(this).outerWidth()) / 2);
                        $('.emoticons').attr('rel', chatRoom.chatRoomId).css({
                            position: "absolute",
                            top: (pos.top - $('.emoticons').outerHeight()) + "px",
                            left: (pos.left - $('.emoticons').outerWidth() + width) + "px"
                        });
                        //.slideToggle(300);
                        e.stopPropagation();
                        showIconTable();
                    });

                    // Check guest Offline
                    if (!isOnline && guestInfo != null) {
                        var newMsg = $('#newmessage-' + chatRoom.chatRoomId);
                        newMsg.val("Liên hệ với khách hàng qua SĐT: " + guestInfo.Phone + " hoặc qua email " + guestInfo.Email);
                        newMsg.prop('disabled', true);
                        newMsg.css({
                            "background-color": "#fff",
                            "font-style": "italic"
                        });
                        autoResizeTextArea(newMsg);
                    }
                }
            };

            chatServer.client.receiveChatMessage = function (chatMessage, chatRoom, isOnline) {
                if (rooms[chatRoom.chatRoomId] == undefined) {
                    return;
                    //chatServer.client.initiateChatUI(chatRoom);
                }
                var chatRoomDiv = $('#chatRoom-' + chatMessage.chatRoomId);
                var chatRoomMessages = $('#messages-' + chatMessage.chatRoomId);
                BDSChatInbox.processDatetime(chatMessage, chatRoomMessages);
                var e;
                if (senderId == chatMessage.senderId) {
                    e = $('#new-message-template-right').tmpl(chatMessage).appendTo(chatRoomMessages);
                } else {
                    e = $('#new-message-template-left').tmpl(chatMessage).appendTo(chatRoomMessages);
                    BDSChatInbox.getAvatar(chatMessage, chatRoom, e);
                    BDSChatInbox.playSound();
                }
                if (typeof (e) != 'undefined') {
                    // Format Emoticons
                    e.emoticons(emoticonOpt);
                    //e[0].scrollIntoView();
                }

                chatRoomMessages.scrollTop(chatRoomMessages[0].scrollHeight);
            };

            chatServer.client.receiveLeftChatMessage = function (chatMessage) {
                var chatRoom = $('#chatRoom-' + chatMessage.chatRoomId);
                var chatRoomMessages = $('#messages-' + chatMessage.chatRoomId);
                var e = $('#new-notify-message-template').tmpl(chatMessage).appendTo(chatRoomMessages);
                //e[0].scrollIntoView();
                //chatRoom[0].scrollIntoView();
            };

            chatServer.client.receiveEndChatMessage = function (chatMessage) {
                // BichTV comment
                //var chatRoom = $('#chatRoom-' + chatMessage.chatRoomId);
                //var chatRoomMessages = $('#messages-' + chatMessage.chatRoomId);
                //var chatRoomText = $('#newmessage-' + chatMessage.chatRoomId);
                //var chatRoomSend = $('#chatsend-' + chatMessage.chatRoomId);
                //var chatRoomEndChat = $('#chatend-' + chatMessage.chatRoomId);

                //var e = $('#new-notify-message-template').tmpl(chatMessage).appendTo(chatRoomMessages);

                //chatRoomText.hide();
                //chatRoomSend.hide();
                //chatRoomEndChat.hide();

                //e[0].scrollIntoView();
                //chatRoom[0].scrollIntoView();

                chatRooms--;
            };

            chatServer.client.updateOnlineContacts = function (chatUsers) {
                var userOnline = { users: chatUsers };
                var e = $('#new-online-contacts').tmpl(userOnline);
                var chatLink = $('#chatLink-' + senderId);
                e.find("#chatLink-" + senderId).remove();
                $("#chatOnlineContacts").html("");
                $("#chatOnlineContacts").html(e);
                BDSChatInbox.updateOnlineUserForRoom(chatUsers);
                BDSChatInbox.updateStatus(BDSChatInbox);
            };

            chatServer.client.receiveErrorMessage = function (errorMessage) {
                if (errorMessage.roomId.toString().length != 0) {
                    var chatRoomMessages = $('#messages-' + errorMessage.chatRoomId);
                    var divError = $('#chat-message-error-template').tmpl(errorMessage);
                    chatRoomMessages.after(divError);
                    var errorHeight = divError.height() + 1;
                    chatRoomMessages.css('height', chatRoomMessages.height() - errorHeight);
                    setTimeout(function() {
                        divError.remove();
                        chatRoomMessages.css('height', chatRoomMessages.height() + errorHeight);
                    }, 3000);
                } else {
                    alert(errorMessage.messageText);
                }
            }

            chatServer.client.receiveHistoryMessage = function(chatMessages, chatRoom, currentTime) {
                if (chatMessages.length > 0) {
                    for (var i = 0; i < chatMessages.length; i++) {
                        BDSChatInbox.loadChatHistoryMessage(chatMessages[i], chatRoom, currentTime);
                    }
                    if (rooms[chatRoom.chatRoomId].isLoadHistory == false) {
                        rooms[chatRoom.chatRoomId].isLoadHistory = true;
                        if($("#messages-" + chatRoom.chatRoomId)[0] != undefined) {
                            $("#messages-" + chatRoom.chatRoomId).scrollTop($("#messages-" + chatRoom.chatRoomId)[0].scrollHeight);    
                        }
                    } else {
                        $("#messages-" + chatRoom.chatRoomId).scrollTop(50);
                    }
                    
                } else {
                    rooms[chatRoom.chatRoomId].isLoadHistory = false;
                }
            }

            chatServer.client.receiveSupporter = function(chatUser) {
                $('.chat-container').find(".chat-with-bds .link-chat-bds").attr("href", "javascript:BDSChatInbox.initiateChat('" + chatUser.UserId + "', '0')");
            };

            chatServer.client.receiveInboxRooms = function (data) {
                BDSChatInbox.initInbox(data);
            };
        }

    };

    this.sendChatMessage = function (chatRoomId) {
        var chatRoomNewMessage = $('#newmessage-' + chatRoomId);

        if (chatRoomNewMessage.val() == null || chatRoomNewMessage.val().trim() == "")
            return;

        var chatMessage = {
            //senderId: senderId,
            //senderName: senderName,
            chatRoomId: chatRoomId,
            //messageText: chatRoomNewMessage.val().replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/\n/g, '<br>')
            messageText: $('<div />').text(chatRoomNewMessage.val()).html().replace(/\n/g, '<br>')
            
        };

        chatRoomNewMessage.val('');
        chatRoomNewMessage.focus();
        try {
            chatServer.server.sendChatMessage(chatMessage).fail(function (e) {
                alert(e);
            });
        } catch (e) {
            //alert(e);
        } 
        

        return false;
    };

    this.closeRoom = function (roomId) {
        delete rooms[parseInt(roomId)];
        BDSChatInbox.endChat(roomId.toString());
    };

    this.endChat = function (chatRoomId) {
        var chatRoomNewMessage = $('#newmessage-' + chatRoomId);

        var chatMessage = {
            senderId: senderId,
            senderName: senderName,
            chatRoomId: chatRoomId,
            messageText: chatRoomNewMessage.val()
        };
        chatRoomNewMessage.val('');
        chatRoomNewMessage.focus();
        chatServer.server.endChat(chatMessage).fail(function (e) {
            //alert(e);
        });
    };

    this.initiateChat = function (toUserId, productId) {
        if (chatServer == null) {
            alert("Problem in connecting to Chat Server. Please Contact Administrator!");
            return;
        }

        chatServer.server.initiateChat(productId, toUserId).fail(function (e) {
            alert(e);
        });
    };

    this.formatName = function(fullName, length){
        var shortName = this.subWords(fullName, length);
        return '<span title="' + fullName + '">' + shortName + '</span>';
    };

    this.isCurrentUser = function(userId){
        return userId == senderId;
    }

    var sepWords = [' ', '-'];

    this.subWords = function (input, length) {
        if (input.length <= length || length <= 0) return input;

        var newContent;
        if (sepWords.indexOf(input[length]) != -1) newContent = input.substring(0, length);
        else {
            var index = length;
            var content = '';
            while (index-- >= 0) {
                if (sepWords.indexOf(input[index]) != -1) {
                    break;
                }
            }
            if (index == 0) newContent = input.substring(0, length);
            else newContent = input.substring(0, index).trim();
        }

        return newContent + '..';
    }

    this.updateOnlineUserForRoom = function(onlineUsers) {
        if (rooms != undefined && rooms != null) {
            for (key in rooms) {
                if (rooms[key].toUserId != undefined) {
                    var statusIcon = $("#chatRoomHeader-" + key + " .user-status");
                    var statusIconBds = $("#chatRoomHeader-" + key + " .link-chat-bds-header");
                    var isOnline = false;
                    for (var i = 0; i < onlineUsers.length; i++) {
                        if (rooms[key].toUserId == onlineUsers[i].UserId) {
                            isOnline = true;
                            break;
                        }
                    }

                    if (isOnline) {
                        statusIcon.addClass("online-status");
                        statusIconBds.removeClass("online-status");
                    } else {
                        statusIcon.removeClass("online-status");
                        statusIconBds.addClass("offline-status");
                    }

                }
            }
        }
    }

    this.getFullName = function (users) {
        for (var i = 0; i < users.length; i++) {
            if (users[i].UserId != senderId) {
                return BDSChatInbox.subWords(users[i].FullName, 30);
            }
        }

        return '';
    }

    this.convertDatetimeToString = function (datetime, type) {
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

        if (type == timeType.Second) {
            return hour + ":" + minute + ":" + second + amOrPm;
        } else if (type == timeType.Minute) {
            return hour + ":" + minute + amOrPm;
        } else if (type == timeType.Hour) {
            return hour + ":" + minute + amOrPm;
        } else if (type == timeType.Day) {
            return hour + ":" + minute + amOrPm + " " + day + " tháng " + month;
        } else if (type == timeType.Month) {
            return hour + ":" + minute + amOrPm + " " + day + " tháng " + month;
        } else if (type == timeType.Year) {
            return hour + ":" + minute + amOrPm + " " + day + " tháng " + month + " năm " + year;
        }

        return hour + ":" + minute + " " + day + " tháng " + month;
    }

    this.processDatetime = function (chatMessage, chatRoomMessages) {
        var dateTime = new Date(chatMessage.timestamp);
        var isAddTime = false;
        var type = timeType.Hour;

        if (rooms[chatMessage.chatRoomId].lastTime == null) {
            rooms[chatMessage.chatRoomId].lastTime = dateTime;
            isAddTime = true;
        } else {
            // check add time and remove time when change time
            if (dateTime.getFullYear() > rooms[chatMessage.chatRoomId].lastTime.getFullYear()) {
                this.removeTimeMessage(timeType.Hour, chatRoomMessages);
                this.removeTimeMessage(timeType.Day, chatRoomMessages);
                this.removeTimeMessage(timeType.Month, chatRoomMessages);
                isAddTime = true;
            } else if (dateTime.getMonth() > rooms[chatMessage.chatRoomId].lastTime.getMonth()) {
                this.removeTimeMessage(timeType.Hour, chatRoomMessages);
                this.removeTimeMessage(timeType.Day, chatRoomMessages);
                isAddTime = true;
            } else if (dateTime.getDate() > rooms[chatMessage.chatRoomId].lastTime.getDate()) {
                this.removeTimeMessage(timeType.Hour, chatRoomMessages);
                isAddTime = true;
            } else if (dateTime > (new Date(rooms[chatMessage.chatRoomId].lastTime.getTime() + (offsetTime * 60 * 1000)))) {
                isAddTime = true;
            }
        }

        if (isAddTime) {
            rooms[chatMessage.chatRoomId].lastTime = dateTime;
            this.addTimeMessage(dateTime, type, chatRoomMessages, true);
        }
    }

    this.addTimeMessage = function (datetime, type, chatRoomMessages, isAppend, messageWelcome) {
        if (isAppend) {
            chatRoomMessages.append("<div class='chat-time chat-time-" + type + "' timestamp='" + datetime.getTime() + "'><div><span>" + this.convertDatetimeToString(datetime, type) + "</span></div></div>");
        } else {
            if (messageWelcome.length == 0) {
                chatRoomMessages.prepend("<div class='chat-time chat-time-" + type + "' timestamp='" + datetime.getTime() + "'><div><span>" + this.convertDatetimeToString(datetime, type) + "</span></div></div>");
            } else {
                messageWelcome.after("<div class='chat-time chat-time-" + type + "' timestamp='" + datetime.getTime() + "'><div><span>" + this.convertDatetimeToString(datetime, type) + "</span></div></div>");
            }
        }

    }

    this.removeTimeMessage = function(type, chatRoomMessages) {
        chatRoomMessages.find("." + classTimeChat + type + ":gt(0)").remove();

        var nextType = type;

        if (type == timeType.Hour) {
            nextType = timeType.Day;
        } else if (type == timeType.Day) {
            nextType = timeType.Month;
        } else if (type == timeType.Month) {
            nextType = timeType.Year;
        }

        var divTime = chatRoomMessages.find("." + classTimeChat + type);

        divTime.find("span").text(this.convertDatetimeToString(new Date(parseInt(divTime.attr('timestamp'))), nextType));
        divTime.removeClass(classTimeChat + type).addClass(classTimeChat + nextType);
    };

    this.getAvatar = function(chatMessage, chatRoom, messaege) {
        var fileId = "";
        for (var i = 0; i < chatRoom.chatUsers.length; i++) {
            if (chatRoom.chatUsers[i].UserId != senderId) {
                fileId = chatRoom.chatUsers[i].Avatar;
            }
        }
        var itemImg = messaege.find(".avatar img");
        itemImg.attr("src", fileId);
    };

    this.highlightChatRoom = function(roomId) {
        if (!$('#newmessage-' + roomId).hasClass("text-focus")) {
            $("#chatPopup-" + roomId + " .chatPopup-header").toggleClass("header-blink");
            if (rooms[roomId].highLightFunc != null) clearTimeout(rooms[roomId].highLightFunc);
            rooms[roomId].highLightFunc = setTimeout('BDSChatInbox.highlightChatRoom("' + roomId + '");', 1000);
        } else {
            if ($("#chatPopup-" + roomId + " .chatPopup-header").hasClass("header-blink")) {
                $("#chatPopup-" + roomId + " .chatPopup-header").removeClass("header-blink");
            }
        }
    };

    this.displayWelComeMessage = function (divChatMessage, chatRoom, message) {
        //var e;
        //var chatMessage = {chatMessageId : 0, messageText: message}
        //e = $('#new-message-template-left').tmpl(chatMessage);

        //divChatMessage.prepend(e);
        //BDSChatInbox.getAvatar(chatMessage, chatRoom, e);
        //if (typeof (e) != 'undefined') {
        //    // Format Emoticons
        //    e.emoticons(emoticonOpt);
        //    //e[0].scrollIntoView();
        //}
    };

    this.loadSupportDetail = function(divChatMessage, chatRoom) {
        var userId = chatRoom.toUserId != senderId ? chatRoom.toUserId : chatRoom.fromUserId;
        for (var i = 0; i < chatRoom.chatUsers.length; i++) {
            if (chatRoom.chatUsers[i].UserId == userId) {
                if (chatRoom.chatUsers[i].IsStaff) {
                    var supportU = $('#chat-caller-bds-template').tmpl({ userId: userId });
                    divChatMessage.before(supportU);
                    this.displayWelComeMessage(divChatMessage, chatRoom, "Rất vui vì đã ghé thăm Batdongsan.com.vn. Bạn có cần tư vấn hoặc giải đáp thắc mắc gì, hãy chat với chúng tôi nhé!");
                    //$.ajax({
                    //    url: handlerUrl + '?action=getavatar&fileid=' + chatRoom.chatUsers[i].Avatar,
                    //    success: function (data) {
                    //        var avatarUrl = defaultSupportAvatar;
                    //        if (data.avatarUrl.length > 0) {
                    //            avatarUrl = data.avatarUrl;
                    //        }
                    //        rooms[chatRoom.chatRoomId].fileId = chatRoom.chatUsers[i].Avatar;
                    //        rooms[chatRoom.chatRoomId].avatarUrl = avatarUrl;
                    //        supportU.find(".support-avatar").attr("src", avatarUrl);
                    //    }
                    //});
                    return true;
                }
            }
        }

        return false;
    };

    this.likeAndDislike = function(userId, isLike) {

    };

    this.addChatArea = function() {
        var divChatContainer = $(document.createElement('div'));
        divChatContainer.addClass('chat-container');
        $("body").append($(document.createElement('div')).addClass("emoticons"));
        divChatContainer.append($("<div style ='display:none' id='chatSound'></div>"));
        $("body").append(divChatContainer);
        divChatContainer.append($("#chat-container-template").tmpl());

    };

    // History 
    this.loadChatHistoryMessage = function (chatMessage, chatRoom, currentTime) {
        var chatRoomDiv = $('#chatRoom-' + chatMessage.chatRoomId);
        var chatRoomMessages = $('#messages-' + chatMessage.chatRoomId);
        var messageWelcome = chatRoomMessages.find('#m-0');
        var e;
        if (senderId == chatMessage.senderId) {
            if (messageWelcome.length != 0) {
                e = $('#new-message-template-right').tmpl(chatMessage);
                messageWelcome.after(e);
            } else {
                e = $('#new-message-template-right').tmpl(chatMessage).prependTo(chatRoomMessages);
            }
        } else {
            if (messageWelcome.length != 0) {
                e = $('#new-message-template-left').tmpl(chatMessage);
                messageWelcome.after(e);
            } else {
                e = $('#new-message-template-left').tmpl(chatMessage).prependTo(chatRoomMessages);
            }
            
            BDSChatInbox.getAvatar(chatMessage, chatRoom, e);
        }

        BDSChatInbox.processDatetimeHistory(messageWelcome, chatMessage, chatRoomMessages, currentTime);
        if (typeof (e) != 'undefined') {
            // Format Emoticons
            e.emoticons(emoticonOpt);

            //e[0].scrollIntoView();
        }
        //chatRoomDiv[0].scrollIntoView();
    }

    this.requestHistory = function (roomId) {
        try {
            var lastMessage = $("#chatRoom-" + roomId + " .message");
            var lastMessageId = 0;
            if (lastMessage.length > 0) {
                if ($(lastMessage[0]).attr("id") != "m-0") {
                    lastMessageId = parseInt($(lastMessage[0]).attr("id").substring(2));
                } else if (lastMessage.length > 1) {
                    lastMessageId = parseInt($(lastMessage[1]).attr("id").substring(2));
                }
            }
            chatServer.server.requestHistory(roomId, lastMessageId);
        } catch (e) {

        }
    }

    this.processDatetimeHistory = function (messageWelcome, chatMessage, chatRoomMessages, currentTime) {
        var dateTime = new Date(chatMessage.timestamp);
        var currentDate = new Date(currentTime);
        var type = timeType.Year;

        // Add time to dic
        if (rooms[chatMessage.chatRoomId].lastTime == null) {
            rooms[chatMessage.chatRoomId].lastTime = dateTime;
        }

        if (dateTime.getFullYear() < currentDate.getFullYear()) {
            type = timeType.Year;
        } else if (dateTime.getMonth() < currentDate.getMonth()) {
            type = timeType.Month;
        } else if (dateTime.getDate() < currentDate.getDate()) {
            type = timeType.Day;
        }
        else {
            type = timeType.Hour;
        }

        this.removeTimeMessageHistory(type, chatRoomMessages, dateTime, chatMessage.chatRoomId);
        this.addTimeMessage(dateTime, type, chatRoomMessages, false, messageWelcome);
    }

    this.removeTimeMessageHistory = function (type, chatRoomMessage, time, roomId) {
        var timeMessages = chatRoomMessage.find(".chat-time");
        if (timeMessages.length > 0) {
            var year = time.getFullYear();
            var month = time.getMonth();
            var day = time.getDate();
            var i = 0;
            var date = null;
            switch (type) {
                case timeType.Year:
                    for (i = 0; i < timeMessages.length; i++) {
                        date = new Date(new Date(parseInt($(timeMessages[i]).attr('timestamp'))));
                        if (date.getFullYear() == year) {
                            $(timeMessages[i]).remove();
                            if (date == rooms[roomId].lastTime) {
                                rooms[roomId].lastTime = time;
                            }
                        }
                    }
                    break;
                case timeType.Month:
                    for (i = 0; i < timeMessages.length; i++) {
                        date = new Date(new Date(parseInt($(timeMessages[i]).attr('timestamp'))));
                        if (date.getFullYear() == year && date.getMonth() == month) {
                            $(timeMessages[i]).remove();
                            if (date == rooms[roomId].lastTime) {
                                rooms[roomId].lastTime = time;
                            }
                        }
                    }
                    break;
                case timeType.Day:
                    for (i = 0; i < timeMessages.length; i++) {
                        date = new Date(new Date(parseInt($(timeMessages[i]).attr('timestamp'))));
                        if (date.getFullYear() == year && date.getMonth() == month && date.getDate() == day) {
                            $(timeMessages[i]).remove();
                            if (date == rooms[roomId].lastTime) {
                                rooms[roomId].lastTime = time;
                            }
                        }
                    }
                    break;
                case timeType.Hour:
                    for (i = 0; i < timeMessages.length; i++) {
                        date = new Date(new Date(parseInt($(timeMessages[i]).attr('timestamp'))));
                        if (time.getTime() - date.getTime() < offsetTime * 60 * 1000) {
                            $(timeMessages[i]).remove();
                            if (date == rooms[roomId].lastTime) {
                                rooms[roomId].lastTime = time;
                            }
                        }
                    }
                    break;
                    //case type.Minute:
                    //    break;
                    //case type.second:
                    //    break;
                default:
                    break;
            }
        }
    };

    this.loadStaff = function() {
        //$.ajax({
        //    url: handlerUrl,
        //    data: { action: "supportstaff" },
        //    success: function (data) {
        //        if (data != undefined && data.length > 0) {
        //            $('.chat-container').find(".chat-with-bds .link-chat-bds").attr("href", "javascript:BDSChatInbox.initiateChat('" + data[0].UserId + "', '0')");
        //        }
        //    }
        //});
        chatServer.server.requestSupporter().fail(function (e) {
            //alert(e);
        });
    }

    this.closeAllRoom = function() {
        for (key in rooms) {
            if (rooms[key].toUserId != undefined) {
                this.closeRoom(key);
            }
        }
    }

    this.playSound = function () {
        $("#chatSound").html("<embed src='" + chatSoundUrl + "' hidden='true' autostart='true' loop='false' class='playSound'>" + "<audio autoplay='autoplay' style='display:none;' controls='controls'><source src='" + chatSoundUrl + "' /></audio>");
    };

    this.requestInboxRooms = function() {
        chatServer.server.requestInboxRooms();
    };

    this.initInbox = function (data) {
        
            for (var i = 0; i < data.length; i++) {
                var group;
                if (data[i].ProductId == 0) {
                    group = $("#chat-user-inbox-pgroup1-Template").tmpl();
                } else {
                    group = $("#chat-user-inbox-pgroup-Template").tmpl({ ProductId: data[i].ProductId });
                    $.ajax({
                        url: handlerUrl + '?action=getproduct&productId=' + data[i].ProductId,
                        success: function (result) {
                            if (result.success) {
                                $("#chat-product-group-" + result.product.ProductId).prepend($('#chat-user-inbox-pgroup-product-template').tmpl(result.product));
                            }
                        }
                    });
                }
                group.appendTo($(".chat-user-mgr-inbox-content").children());

                for (var j = 0; j < data[i].Rooms.length; j++) {
                    if (data[i].Rooms[j].ChatMessage != null) {
                        var e = $("#chat-user-inbox-rinfo-Template").tmpl({
                            Email: (data[i].Rooms[j].Room.ClientFromUserId == senderId ? data[i].Rooms[j].Room.ToFullName : data[i].Rooms[j].Room.FromFullName),
                            Message: data[i].Rooms[j].ChatMessage.messageText,
                            Time: BDSChatInbox.convertTime(data[i].Rooms[j].ChatMessage.timestamp, (new Date()).getTime()),
                            ClassInbox: data[i].Rooms[j].ChatMessage.senderId == senderId ? "outbox" : "",
                            RoomId: data[i].Rooms[j].Room.ChatRoomId,
                            ProductId: data[i].Rooms[j].Room.ProductId,
                            UserId: data[i].Rooms[j].Room.ClientFromUserId == senderId ? data[i].Rooms[j].Room.ClientToUserId : data[i].Rooms[j].Room.ClientFromUserId,
                        });
                        e.appendTo(group.find("ul"));
                        e.click(function () {
                            var roomId = $(this).attr("roomId");
                            if ($("#chatPopup-" + roomId).length == 0) {
                                var userId = $(this).attr("userId");
                                var productId = $(this).attr("productId");
                                BDSChatInbox.initiateChat(userId, productId);
                            }
                        });
                        e.emoticons(emoticonOpt);
                    }
                }
        }
    };

    this.addChatArea = function (chatRoomId, title, fullName, chatArea) {
        // Xóa room cũ
        this.removeRoom();

        var divChatPopup = $("#chat-inbox-popup-template").tmpl({ id: chatRoomId, fullName: fullName }).appendTo($('.chat-user-mgr-chat-area'));
        $("#chat-inbox-popup-header-template").tmpl().appendTo(divChatPopup);
        chatArea.appendTo(divChatPopup);
        divChatPopup.find(".chatPopup-header .header-title").append(title);

        // Hard style
        divChatPopup.find(".chatPopup-header").css({
            "background-color": "#ededed",
            "border-bottom": "1px solid #999999",
            "color": "#000",
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
            "max-width": "342px"
        });

        divChatPopup.find(".chat-command").css({
            "height": "58px"
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
            "min-height": "54px"
        });
    }

    this.removeRoom = function () {
        var room = $(".chat-user-mgr .chat-user-mgr-chat-area").children();
        if (room.length > 0) {
            BDSChatInbox.closeRoom(room.attr("roomid"));
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

    this.updateLastestMessage = function (chatMessage) {

    };

    this.updateUnreadMessage = function () {

    };

    this.addRoomToInbox = function (chatRoom) {

    };

    this.convertTime = function (time, currentTime) {
        var datetime = new Date(time);
        var currentDatetime = new Date(currentTime);

        var year = datetime.getFullYear();
        var month = datetime.getMonth() + 1;
        var day = datetime.getDate() < 10 ? ("0" + datetime.getDate()) : datetime.getDate();
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

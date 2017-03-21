var chatServerUrl = "";
var senderId = '';
var senderName = '';
var productId = 0;
var chatPK = '';

BDSChat = new function () {
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
    
    var cookieRoomName = "CHAT_ROOM";
    var roomHistories = null;
    var classTimeChat = "chat-time-";
    var defaultAvatar = "/images/no-avatar.png";
    var chatSoundUrl = "/Scripts/Chats/ChatSound.mp3";
    var offsetTime = 60; // 1h * 60
    var isAdmin = false;
    var isUserMgr = false;

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
            BDSChat.closeAllRoom();
        }
    };

    this.attachEvents = function () {
        BDSChat.addChatArea();
        var $this = this;
        //$("#userNameLabel").html(senderName);
        if ($.connection != null) {
            $.connection.hub.url = '/signalr/hubs';
            //$.connection.hub.qs = "__ui=" + encodeURIComponent(senderId) + "&__un=" + encodeURIComponent(senderName);
            //$.connection.hub.qs = "__ui=" + encodeURIComponent(senderId) + "&__un=" + encodeURIComponent(senderName) + (chatPK != null && chatPK != '' && isAdmin ? "&__pk=" + encodeURIComponent(chatPK) : '');
            chatServer = $.connection.chatServer;
            $.connection.hub.start({ transport: 'auto' }, function () {
                //BDSChat.loadRoomFromCookie();
                //chatServer.server.connect(senderId, senderName).fail(function (e) {
                //    alert(e);
                //});
            }).fail(function () {
                //alert("error connect");
            }).done(function () {
                //alert("connect");
                BDSChat.RequestUsers();
                BDSChat.loadRoomFromCookie();
                BDSChat.loadUnreadMessageForMenu();
            });;

            chatServer.client.initiateChatUI = function (chatRoom, isOnline, totalUnread) {
                var chatRoomDiv = $('#chatRoom-' + chatRoom.chatRoomId);
                //if (($(chatRoomDiv).length > 0)) {
                if (rooms[chatRoom.chatRoomId] != undefined) {
                    if (rooms[chatRoom.chatRoomId].popup != null) {
                        rooms[chatRoom.chatRoomId].popup.displayRoomExternal(chatRoom.chatRoomId);
                    }
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
                        highLightFunc: null,
                        productId: chatRoom.productId,
                        isLoadHistory: true,
                        totalUnread: totalUnread
                        //isMaximize: true,
                };

                    var e = $('#new-chatroom-template').tmpl(chatRoom);
                    var c = $('#new-chat-header').tmpl(chatRoom);

                    var divChatMessage = e.find('.chatMessages');

                    // load avatar
                    // BDSChat.getAvatar(null, chatRoom, null);
                    chatRooms++;

                    if (!isUserMgr) {
                        rooms[chatRoom.chatRoomId].popup = e.chatPopup({
                            "id": chatRoom.chatRoomId,
                            "fullName": BDSChat.getFullName(chatRoom.chatUsers),
                            "title": c,
                            "close": function(roomId) {
                                //delete rooms[parseInt(roomId)];
                                //BDSChat.setCookie();
                                //BDSChat.endChat(roomId.toString());
                                BDSChat.closeRoom(roomId);
                            }
                        });
                    } else {
                        BDSInboxChat.addChatArea(chatRoom.chatRoomId, c, BDSChat.getFullName(chatRoom.chatUsers), e);
                    }

                    //rooms[chatRoom.chatRoomId].popup = e.chatPopup({
                    //    "id": chatRoom.chatRoomId,
                    //    "fullName": BDSChat.getFullName(chatRoom.chatUsers),
                    //    "title": c,
                    //    "close": function (roomId) {
                    //        //delete rooms[parseInt(roomId)];
                    //        //BDSChat.setCookie();
                    //        //BDSChat.endChat(roomId.toString());
                    //        BDSChat.closeRoom(roomId);
                    //    }
                    //});

                    // Add online User
                    if (isOnline) {
                        c.find(".user-status").addClass("online-status");
                        c.find(".link-chat-bds-header").removeClass("offline-status");
                    } else {
                        c.find(".user-status").removeClass("online-status");
                        c.find(".link-chat-bds-header").addClass("offline-status");
                    }
                    
                    // update popup minimize or maximize
                    BDSChat.loadMaximizeOrMinimize(chatRoom.chatRoomId, toUserId);

                    // Get history
                    rooms[chatRoom.chatRoomId].isLoadHistory = false;
                    BDSChat.requestHistory(chatRoom.chatRoomId);


                    $('#newmessage-' + chatRoom.chatRoomId).focus(function () {
                        if (!$(this).hasClass("text-focus")) {
                            $(this).addClass("text-focus");
                        }
                        
                        var roomId = parseInt($(this).attr("id").split("-")[1]);
                        if (rooms[roomId] != undefined && rooms[roomId].popup != null) {
                            rooms[roomId].popup.addUnreadMessageExternal(roomId, 0);
                        }

                        // Update read message for room by roomId and sender id 
                        if (roomHistories == null || roomHistories.length == 0) {
                            //console.log(roomId + '-------------' + roomHistories.length);
                            chatServer.server.updateReadMessage(roomId);
                        }
                        
                    });

                    $('#newmessage-' + chatRoom.chatRoomId).blur(function () {
                        if ($(this).hasClass("text-focus")) {
                            $(this).removeClass("text-focus");
                        }
                    });

                    $('#newmessage-' + chatRoom.chatRoomId).focus();

                    $("#messages-" + chatRoom.chatRoomId).scroll(function (event) {
                        if ($(this).scrollTop() == 0) {
                            var roomId = parseInt($(this).attr("id").split("-")[1]);
                            if (rooms[roomId].isLoadHistory) {
                                BDSChat.requestHistory(roomId);
                            }
                        }
                        event.preventDefault();
                        return false;
                    });

                    $('#sendmessage-' + chatRoom.chatRoomId).keypress(function (e) {
                        if ((e.which && e.which == 13) || (e.keyCode && e.keyCode == 13)) {
                            //$('#chatsend-' + chatRoom.chatRoomId).click();
                            BDSChat.sendChatMessage(chatRoom.chatRoomId);
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
                    
                    // Set cookie
                    BDSChat.setCookie();
                }
            };

            //chatServer.client.updateChatUI = function (chatRoom) {
            //    var chatRoomHeader = $('#chatRoomHeader-' + chatRoom.chatRoomId);
            //    var c = $('#new-chat-header').tmpl(chatRoom);
            //    chatRoomHeader.html(c);
            //};

            chatServer.client.receiveChatMessage = function (chatMessage, chatRoom, isOnline) {
                //gắn thẻ a vào link in message chat
                var reLinkInMess = /http:\/\/.*[^\s+$]/.exec(chatMessage.messageText);
                if (reLinkInMess != null && reLinkInMess.length > 0) {
                    chatMessage.messageText = chatMessage.messageText.replace(reLinkInMess, '<a target="_blank" href="' + reLinkInMess + '">' + reLinkInMess + '</a>');
                }
                
                if (rooms[chatRoom.chatRoomId] == undefined) {
                    chatServer.client.initiateChatUI(chatRoom, isOnline, null);
                    
                    // Kiểm tra nếu load history có rồi thì không hiển thị tin nhắn nữa
                    if ($("#m-" + chatMessage.chatMessageId).length > 0) {
                        return;
                    }
                }
                var chatRoomDiv = $('#chatRoom-' + chatMessage.chatRoomId);
                var chatRoomMessages = $('#messages-' + chatMessage.chatRoomId);
                BDSChat.processDatetime(chatMessage, chatRoomMessages);
                var e;
                if (senderId == chatMessage.senderId) {
                    e = $('#new-message-template-right').tmpl(chatMessage).appendTo(chatRoomMessages);
                } else {
                    e = $('#new-message-template-left').tmpl(chatMessage).appendTo(chatRoomMessages);
                    BDSChat.getAvatar(chatMessage, chatRoom, e);

                    // Mark unread message
                    var isUnread = false;
                    if (rooms[chatMessage.chatRoomId].popup != null && (!rooms[chatMessage.chatRoomId].popup.hasClass("show") || rooms[chatMessage.chatRoomId].popup.hasClass("chatPopup-header-maximinze"))) {
                        isUnread = true;
                    } else {
                        chatServer.server.updateReadMessage(chatRoom.chatRoomId);
                    }

                    if (isUnread) {
                        // highlight popup
                        if (rooms[chatMessage.chatRoomId].highLightFunc != null) clearTimeout(rooms[chatMessage.chatRoomId].highLightFunc);
                        rooms[chatMessage.chatRoomId].highLightFunc = setTimeout('BDSChat.highlightChatRoom("' + chatMessage.chatRoomId + '");', 1000);

                        rooms[chatMessage.chatRoomId].totalUnread += 1;
                        if (rooms[chatMessage.chatRoomId] != undefined && rooms[chatMessage.chatRoomId].popup != null) {
                            rooms[chatMessage.chatRoomId].popup.addUnreadMessageExternal(chatRoom.chatRoomId, rooms[chatMessage.chatRoomId].totalUnread);
                        }
                    }

                    BDSChat.playSound();
                }
                if (typeof (e) != 'undefined') {
                    // Format Emoticons
                    e.emoticons(emoticonOpt);
                    //e[0].scrollIntoView();
                    //e.scrollTop(e[0].scrollHeight);
                }

                //chatRoomDiv[0].scrollIntoView(); // not use scrollIntoView because this will error in chrome: scroll div -> scroll window
                chatRoomMessages.scrollTop(chatRoomMessages[0].scrollHeight);
            };

            chatServer.client.receiveLeftChatMessage = function (chatMessage) {
                var chatRoom = $('#chatRoom-' + chatMessage.chatRoomId);
                var chatRoomMessages = $('#messages-' + chatMessage.chatRoomId);
                var e = $('#new-notify-message-template').tmpl(chatMessage).appendTo(chatRoomMessages);
                e[0].scrollIntoView();
                chatRoom[0].scrollIntoView();
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

            chatServer.client.updateOnlineContacts = function (user) {
                BDSChat.updateStatusUserForRoom(user, true);
            };

            chatServer.client.updateOfflineContacts = function (user) {
                BDSChat.updateStatusUserForRoom(user, false);
            };

            chatServer.client.receiveErrorMessage = function(errorMessage) {
                //if (errorMessage.roomId.toString().length != 0) {
                //    var chatRoomMessages = $('#messages-' + errorMessage.chatRoomId);
                //    var divError = $('#chat-message-error-template').tmpl(errorMessage);
                //    chatRoomMessages.after(divError);
                //    var errorHeight = divError.height() + 1;
                //    chatRoomMessages.css('height', chatRoomMessages.height() - errorHeight);
                //    setTimeout(function() {
                //        divError.remove();
                //        chatRoomMessages.css('height', chatRoomMessages.height() + errorHeight);
                //    }, 3000);
                //} else {
                //    alert(errorMessage.messageText);
                //}
            };

            chatServer.client.receiveHistoryMessage = function(chatMessages, chatRoom, currentTime) {
                if (chatMessages.length > 0) {
                    for (var i = 0; i < chatMessages.length; i++) {
                        BDSChat.loadChatHistoryMessage(chatMessages[i], chatRoom, currentTime);
                    }
                    if (rooms[chatRoom.chatRoomId].isLoadHistory == false) {
                        rooms[chatRoom.chatRoomId].isLoadHistory = true;
                        if ($("#messages-" + chatRoom.chatRoomId)[0] != undefined) {
                            $("#messages-" + chatRoom.chatRoomId).scrollTop($("#messages-" + chatRoom.chatRoomId)[0].scrollHeight);
                        }
                    } else {
                        $("#messages-" + chatRoom.chatRoomId).scrollTop(50);
                    }

                } else {
                    rooms[chatRoom.chatRoomId].isLoadHistory = false;
                }
            };

            chatServer.client.updateOnlineUsers = function(users) {
                if (users != null && users.length > 0) {
                    var userContainer = $(".list-user-container .list-user ul");
                    for (var i = 0; i < users.length; i++) {
                        userContainer.append($("#chat-list-user").tmpl(users[i]));
                    }

                    $(".list-user").slimScroll();
                }
            };
            
            chatServer.client.updateReadMessage = function (roomId) {
                if (rooms[roomId] != undefined && rooms[roomId].popup != null) {
                    rooms[roomId].totalUnread = 0;
                    rooms[roomId].popup.addUnreadMessageExternal(roomId, 0);
                }
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
        //chatRoomNewMessage.focus();
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
        BDSChat.setCookie();
        BDSChat.endChat(roomId.toString());
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
        chatServer.server.endChat(chatRoomId).fail(function (e) {
            //alert(e);
        });
    };

    this.initiateChat = function (toUserId) {
        if (chatServer == null) {
            alert("Problem in connecting to Chat Server. Please Contact Administrator!");
            return;
        }

        chatServer.server.initiateChat(toUserId).fail(function (e) {
            alert(e);
        });
    };

    this.formatName = function(fullName, length){
        var shortName = this.subWords(fullName, length);
        return '<span title="' + fullName + '">' + shortName + '</span>';
    };

    this.isCurrentUser = function(userId) {
        return userId == senderId;
    };

    var sepWords = [' ', '-'];

    this.subWords = function(input, length) {
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
    };

    this.RequestUsers = function() {
        chatServer.server.requestOnlineUsers();
    };

    this.updateStatusUserForRoom = function (user, isOnline) {
        if (rooms != undefined && rooms != null) {
            for (key in rooms) {
                if (rooms[key].toUserId != undefined) {
                    if (rooms[key].toUserId == user.UserId) {
                        var statusIcon = $("#chatRoomHeader-" + key + " .user-status");
                        if (isOnline) {
                            statusIcon.addClass("online-status");
                        } else {
                            statusIcon.removeClass("online-status");
                        }

                        // update collapse room
                        if (rooms[key].popup != null) {
                            rooms[key].popup.updateStatusExternal(key, isOnline);
                        }

                    }
                }
            }
        }

        // Update status for list users
        var users = $(".list-user li");
        for (var i = 0; i < users.length; i++) {
            if ($(users[i]).attr("userId") == user.UserId) {
                if (isOnline) {
                    $(users[i]).removeClass("offline-status");
                } else {
                    $(users[i]).addClass("offline-status");
                }
                break;
            }
        }
    };

    this.getFullName = function(users) {
        for (var i = 0; i < users.length; i++) {
            if (users[i].UserId != senderId) {
                return BDSChat.subWords(users[i].FullName, 30);
            }
        }
        return '';
    };

    this.convertDatetimeToString = function(datetime, type) {
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
    };

    this.processDatetime = function(chatMessage, chatRoomMessages) {
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
    };

    this.addTimeMessage = function(datetime, type, chatRoomMessages, isAppend, messageWelcome) {
        if (isAppend) {
            chatRoomMessages.append("<div class='chat-time chat-time-" + type + "' timestamp='" + datetime.getTime() + "'><div><span>" + this.convertDatetimeToString(datetime, type) + "</span></div></div>");
        } else {
            if (messageWelcome.length == 0) {
                chatRoomMessages.prepend("<div class='chat-time chat-time-" + type + "' timestamp='" + datetime.getTime() + "'><div><span>" + this.convertDatetimeToString(datetime, type) + "</span></div></div>");
            } else {
                messageWelcome.after("<div class='chat-time chat-time-" + type + "' timestamp='" + datetime.getTime() + "'><div><span>" + this.convertDatetimeToString(datetime, type) + "</span></div></div>");
            }
        }

    };

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
            rooms[roomId].highLightFunc = setTimeout('BDSChat.highlightChatRoom("' + roomId + '");', 1000);
        } else {
            if ($("#chatPopup-" + roomId + " .chatPopup-header").hasClass("header-blink")) {
                $("#chatPopup-" + roomId + " .chatPopup-header").removeClass("header-blink");
            }
        }
    };

    this.loadRoomFromCookie = function() {
        roomHistories = BDSChat.getCookie();
        if (roomHistories.length > 0) {
            $(".chat-container").hide();
            BDSChat.initiateChat(roomHistories[0].toUserId, roomHistories[0].productId);
        }
    };

    this.loadMaximizeOrMinimize = function(roomId, toUserId) {
        if (roomHistories != null && roomHistories.length > 0) {
            for (var i = 0; i < roomHistories.length; i++) {
                if (roomHistories[i].toUserId == toUserId) {
                    if (rooms[roomId] != undefined && rooms[roomId].popup != null) {
                        rooms[roomId].popup.minimizeOrMaximizeExternal(roomId, roomHistories[i].isMaximize);
                    }

                    roomHistories.remove(roomHistories[i]);

                    if (roomHistories != null && roomHistories.length > 0) {
                        BDSChat.initiateChat(roomHistories[0].toUserId, roomHistories[0].productId);
                    } else {
                        // Mark to read for last room display by cookie
                        // TODO...
                        $(".chat-container").show();
                    }

                    break;
                }
            }
        }
    };

    this.addChatArea = function() {
        var divChatContainer = $(document.createElement('div'));
        divChatContainer.addClass('chat-container');
        $("body").append($(document.createElement('div')).addClass("emoticons"));
        divChatContainer.append($("<div style ='display:none' id='chatSound'></div>"));
        $("body").append(divChatContainer);
    };

    this.getCookie = function () {
        var roomStr = getCookie(cookieRoomName);
        var roomInfos = [];
        if (roomStr != undefined && roomStr.length > 0) {
            var roomArr = roomStr.split('|');
            if (roomArr.length > 1 && roomArr[0] == senderId) {
                for (var i = 1; i < roomArr.length; i++) {
                    var roomInfoArr = roomArr[i].split(',');
                    if (roomInfoArr.length == 3) {
                        try {
                            roomInfos.push({ toUserId: roomInfoArr[0], productId: roomInfoArr[1], isMaximize: roomInfoArr[2] != 'false' });
                        } catch (e) {
                        }
                    }
                }
            }
        }

        return roomInfos;
    };

    this.setCookie = function() {
        var roomIds = [];
        var collpseItems = $(".collapse-room-item");
        if (collpseItems.length > 0) {
            for (var i = 0; i < collpseItems.length; i++) {
                roomIds.push($(collpseItems[i]).attr("roomId"));
            }
        }

        if ($.fn.chatPopupShowList != undefined && $.fn.chatPopupShowList != null && $.fn.chatPopupShowList.length > 0) {
            for (var j = 0; j < $.fn.chatPopupShowList.length; j++) {
                roomIds.push($.fn.chatPopupShowList[j]);
            }
        }

        if (roomIds.length > 0) {
            var cookieVal = senderId;
            for (var k = 0; k < roomIds.length; k++) {
                var room = rooms[parseInt(roomIds[k])];
                cookieVal += "|";
                cookieVal += room.toUserId + "," + room.productId + "," + !$("#chatPopup-" + roomIds[k]).hasClass("chatPopup-header-maximinze");
            }
            setCookie(cookieRoomName, cookieVal);
        } else {
            setCookie(cookieRoomName, "", 1);
        }
    };

    // History 
    this.loadChatHistoryMessage = function (chatMessage, chatRoom, currentTime) {
        
        //gắn thẻ a vào link in message chat
        var reLinkInMess = /http:\/\/.*[^\s+$]/.exec(chatMessage.messageText);
        if (reLinkInMess != null && reLinkInMess.length > 0) {
            chatMessage.messageText = chatMessage.messageText.replace(reLinkInMess, '<a target="_blank" href="' + reLinkInMess + '">' + reLinkInMess + '</a>');
        }

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

            BDSChat.getAvatar(chatMessage, chatRoom, e);
        }

        BDSChat.processDatetimeHistory(messageWelcome, chatMessage, chatRoomMessages, currentTime);
        if (typeof(e) != 'undefined') {
            // Format Emoticons
            e.emoticons(emoticonOpt);

            //e[0].scrollIntoView();
        }
        //chatRoomDiv[0].scrollIntoView();
    };

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

    this.processDatetimeHistory = function(messageWelcome, chatMessage, chatRoomMessages, currentTime) {
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
        } else {
            type = timeType.Hour;
        }

        this.removeTimeMessageHistory(type, chatRoomMessages, dateTime, chatMessage.chatRoomId);
        this.addTimeMessage(dateTime, type, chatRoomMessages, false, messageWelcome);
    };

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

    this.setIsAdmin = function(isAdminValue) {
        isAdmin = isAdminValue;
    };

    this.setIsUserMgr = function(isUserMgrValue) {
        isUserMgr = isUserMgrValue;
    };

    this.closeAllRoom = function() {
        for (key in rooms) {
            if (rooms[key].toUserId != undefined) {
                BDSChat.endChat(key);
            }
        }
    };

    this.playSound = function () {
        $("#chatSound").html("<embed src='" + chatSoundUrl + "' hidden='true' autostart='true' loop='false' class='playSound'>" + "<audio autoplay='autoplay' style='display:none;' controls='controls'><source src='" + chatSoundUrl + "' /></audio>");
    };

    this.loadUnreadMessageForMenu = function() {
        $.ajax({
            url: "/chat/GetUnreadChatMessage",
            success: function(data) {
                if (data.length > 0) {
                    $(".messages-menu .label-success").text(data.length);
                    var container = $("#quick-inbox-container-template").tmpl({ TotalCount: data.length });
                    container.appendTo(".messages-menu");
                    var ul = container.find("ul.menu");
                    for (var i = 0; i < data.length; i++) {
                        var item = {
                            UserId: data[i].User.UserId,
                            Avatar: data[i].User.Avatar,
                            FullName: data[i].User.FullName,
                            Time: BDSChat.convertTime(data[i].ChatMessage.timestamp, new Date().getTime()),
                            Message: data[i].ChatMessage.messageText
                        };

                        var message = $("#quick-inbox-item-template").tmpl(item);
                        message.emoticons(emoticonOpt);
                        ul.append(message);
                    }

                    $(".messages-menu .dropdown-toggle").click(function() {
                        $(".messages-menu .label-success").text("");
                    });
                }
            }
        });
    };

    this.quickInboxItemClick = function(userId, productId) {
        BDSChat.initiateChat(userId, productId);
        $(".chat-quick-inbox-content").removeClass("active");
    };

    this.convertTime = function(time, currentTime) {
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
        } else {
            return hour + ":" + minute + amOrPm;
        }
    };

};

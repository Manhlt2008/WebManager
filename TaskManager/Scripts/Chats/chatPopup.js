// store list popup id
(function ($) {
    var chatPopup = function (options) {
        var opts = $.extend({}, $.fn.chatPopup.defaults, options);
        var idChatPopup = "#chatPopup-" + opts.id;
        var chatArea = $(".chat-container");
        var chatroom = $(this);

        var init = function () {
            var right = opts.offset;
            if ($.fn.chatPopupList > (opts.maxPopup - 1)) {
                processRoom();
            } else if ($.fn.chatPopupList == 0) {
                // Khởi tạo collapse-chat-room
                initCollapseRoom();
            }

            var divChatPopup = $("#chat-popup-template").tmpl({ id: opts.id, fullName: opts.fullName })
	            .css({
	                "width": opts.width,
	                "bottom": '0',
	                "right": right,
	                "position": "absolute"
	            })
	            .appendTo(chatArea);
            $("#chat-popup-header-template").tmpl().appendTo(divChatPopup);
            chatroom.appendTo(divChatPopup);
            $(idChatPopup + " .chatPopup-header .header-title").append(opts.title);
            $.fn.chatPopupList += 1;

            //Event
            $(idChatPopup + " .header-icon .header-icon-close").click(function (e) {
                closeChatRoom(opts.id);
                e.stopPropagation();
                return false;
            });

            $(idChatPopup + " .header-icon .header-icon-minimize").click(function (e) {
                if ($(idChatPopup).hasClass("chatPopup-header-maximinze")) {
                    maximizeOrMinimizePopup(opts.id, true);
                    $(idChatPopup + " .header-icon .header-icon-minimize i").addClass("fa-minus");
                    $(idChatPopup + " .header-icon .header-icon-minimize i").removeClass("fa-external-link");
                } else {
                    maximizeOrMinimizePopup(opts.id, false);
                    $(idChatPopup + " .room-setting").removeClass("visible");
                    $(idChatPopup + " .header-icon .header-icon-minimize i").removeClass("fa-minus");
                    $(idChatPopup + " .header-icon .header-icon-minimize i").addClass("fa-external-link");
                }
                
                e.stopPropagation();
                return false;
            });
            
            $(idChatPopup + " .chatPopup-header").click(function () {
                if ($(idChatPopup).hasClass("chatPopup-header-maximinze")) {
                    maximizeOrMinimizePopup(opts.id, true);
                    $(idChatPopup + " .header-icon .header-icon-minimize i").addClass("fa-minus");
                    $(idChatPopup + " .header-icon .header-icon-minimize i").removeClass("fa-external-link");
                } else {
                    maximizeOrMinimizePopup(opts.id, false);
                    $(idChatPopup + " .room-setting").removeClass("visible");
                    $(idChatPopup + " .header-icon .header-icon-minimize i").removeClass("fa-minus");
                    $(idChatPopup + " .header-icon .header-icon-minimize i").addClass("fa-external-link");
                }
            });

            $(idChatPopup + " .header-icon .header-icon-setting").click(function () {
                $(idChatPopup + " .room-setting").toggleClass("visible");
            });

            showRoomById(opts.id);
            autoPosition();
        };

        // process when new room created
        var processRoom = function() {
            var first = $("#chatPopup-" + $.fn.chatPopupShowList[0]);
            first.removeClass("show");
            var roomId = $.fn.chatPopupShowList[0];
            var fullName = getFullName(first);
            $.fn.chatPopupShowList.remove(roomId);
            addCollapseItem(roomId, fullName);
        };

        // show room from collapse room
        var showCollapeRoom = function (roomId) {
            processRoom();
            showRoomById(roomId);
            autoPosition();
        }

        // display room by id
        var showRoomById = function (roomId) {
            $("#chatPopup-" + roomId).addClass("show");
            $.fn.chatPopupShowList.push(roomId.toString());
            maximizeOrMinimizePopup(roomId, true);
        };

        // process close chat room
        var closeChatRoom = function (roomId) {
            $("#chatPopup-" + roomId).remove();
            $.fn.chatPopupList -= 1;
            $.fn.chatPopupShowList.remove(roomId.toString());

            // Nếu room hiển thị nhỏ hơn maxPopup và nhỏ hơn room đang có thì load room collapse
            if ($.fn.chatPopupShowList.length < opts.maxPopup && $.fn.chatPopupList > $.fn.chatPopupShowList.length) {
                var first = $(".collapse-room-item:first");
                if (first != undefined) {
                    var colroomId = getRoomId(first);
                    removeCollapseItem(colroomId);
                    showRoomById(colroomId);
                }
            }
            autoPosition();
            opts.close(roomId);

            // Xóa collapse-chat-room khi không còn room nào
            if ($.fn.chatPopupList == 0) {
                $(".collapse-chat-room").remove();
            }
        }

        // init collapse room
        var initCollapseRoom = function () {
            $('#chat-collapse-room-template').tmpl().appendTo(chatArea);

            $('.link-collapse-room').click(function () {
                $('.collapse-chat-room-relative').toggleClass('active');
            });

            $(document).click(function () {
                displayItemCollapse(false);
            });

            $(".collapse-chat-room").click(function (e) {
                e.stopPropagation(); // This is the preferred method.
                return false;        // This should not be used unless you do not want
                // any click events registering inside the div
            });
        };

        // Add collapse item
        var addCollapseItem = function (roomId, fullName) {
            var item = $("#chat-collapse-room-item-template").tmpl({ roomId: roomId, fullName: fullName });
            item.appendTo($('.collapse-room-list'));

			//  update status 
	        updateStatus(roomId, $("#chatRoomHeader-" + roomId + " .user-status").hasClass("online-status"));
			
            // close room
            item.find(".collapse-room-item-close").click(function () {
                collapseItemCloseClick(this);
            });

            // open room
            item.click(function () {
                collapseItemClick(this);
            });
        };

        // Auto position for all popup
        var autoPosition = function () {
            for (var i = 0; i < $.fn.chatPopupShowList.length; i++) {
                $("#chatPopup-" + $.fn.chatPopupShowList[i]).css("right", (opts.width + opts.offset) * i + opts.right);
            }

            if ($.fn.chatPopupList > opts.maxPopup) {
                $(".collapse-chat-room").show();
                $(".collapse-chat-room").css("right", (opts.width + opts.offset) * opts.maxPopup + opts.right + 205);
            } else {
                $(".collapse-chat-room").hide();
            }

            // Count collapse item
            calCollapseCount();
        };

        // handle event click close room from collapse item
        var collapseItemCloseClick = function (control) {
            var roomid = getRoomId(control);
            removeCollapseItem(roomid);
            closeChatRoom(roomid);
            displayItemCollapse(false);
        };

        // update collapse items number
        var calCollapseCount = function () {
            $(".link-collapse-room-number").text($(".collapse-room-item").length);
        };

        // handle event click collpase item to display popup
        var collapseItemClick = function(control) {
            var roomId = getRoomId(control);
            removeCollapseItem(roomId);
            showCollapeRoom(roomId);
            displayItemCollapse(false);
        };

        // delete collapse item
        var removeCollapseItem = function (roomId) {
            $('#collapse-room-item-' + roomId).remove();
        };

        // Get attribute roomId
        var getRoomId = function(control) {
            return $(control).attr("roomId");
        };

        // Get attribute fullName
        var getFullName = function(control) {
            return $(control).attr("fullName");
        };

        //Display collapse
        var displayItemCollapse = function (isDisplay) {
            if (isDisplay) {
                $('.collapse-chat-room-relative').addClass('active');
            } else {
                $('.collapse-chat-room-relative').removeClass('active');
            }
        };

        // maximum popup
        var maximizeOrMinimizePopup = function(roomId, isMaximize) {
            if (isMaximize) {
                $("#chatPopup-" + roomId).removeClass("chatPopup-header-maximinze");
                $('#messages-' + roomId).scrollTop($('#messages-' + roomId).prop('scrollHeight'));
                $('#newmessage-' + roomId).focus();
            } else {
                $("#chatPopup-" + roomId).addClass("chatPopup-header-maximinze");
            }
            updateCookie();
        };

        // update cookie
        var updateCookie = function() {
            BDSChat.setCookie();
        };

        // Get number of popup
        var getLengthPopupList = function() {
            return $(".chatPopup").length;
        };

		var updateStatus = function(roomId, isOnline) {
            if (isOnline) {

                $("#collapse-room-item-" + roomId + " a").addClass("user-online");
            } else {
                $("#collapse-room-item-" + roomId + " a").removeClass("user-online");
            }
        };
        // ============  public method ===========

        // display room
        this.displayRoomExternal = function (roomId) {
            if ($.inArray(roomId.toString(), $.fn.chatPopupShowList) == -1) {
                removeCollapseItem(roomId);
                showCollapeRoom(roomId);
            } else {
                maximizeOrMinimizePopup(roomId, true);
            }
        };

        this.minimizeOrMaximizeExternal = function (roomId, isMaximize) {
            maximizeOrMinimizePopup(roomId, isMaximize);
        };

        // Update status
        this.updateStatusExternal = function (roomId, isOnline) {
            updateStatus(roomId, isOnline);
        };

        // Update message unread
	    this.addUnreadMessageExternal = function (roomId, count) {
	        var unreadItem = $("#collapse-room-item-" + roomId + " .unread-count");
	        var unreadHeader = $("#chatRoomHeader-" + roomId + " .unread-count");
	        if (unreadItem.length > 0) {
	            if (count == 0) {
	                unreadItem.text("");
	                unreadHeader.text("");
	            } else {
	                unreadItem.text("(" + count + ")");
	                unreadHeader.text("(" + count + ")");
	            }
	        }
	        if (unreadHeader.length > 0) {
	            if (count == 0) {
	                unreadHeader.text("");
	            } else {
	                unreadHeader.text("(" + count + ")");
	            }
	        }
	        //updateRoomUnread();
	    };
        // Init popup chat
        init();

        return this;
    };
    $.fn.chatPopup = chatPopup;
    $.fn.chatPopupShowList = new Array();
    $.fn.chatPopupList = 0;

    // Plugin defaults – added as a property on our plugin function.
    $.fn.chatPopup.defaults = {
        id: '',
        fullName: '',
        title: '',
        offset: 5,
        right: 5,
        maxPopup: 3,
        width: 260,
        close: null
    };
}(jQuery));

// array remove function
Array.prototype.remove = function () {
    var what, a = arguments, l = a.length, ax;
    while (l && this.length) {
        what = a[--l];
        while ((ax = this.indexOf(what)) !== -1) {
            this.splice(ax, 1);
        }
    }
    return this;
};
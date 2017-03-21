// Init chat icons
var emoSetting = {
    path: '/images/emoticons/',
    parent: '#pnlEmoticions',
    selIconCallback: function (chatId, iconCode) {
        var oldMsg = $('#newmessage-' + chatId).val().trim();
        if (oldMsg == '') oldMsg = iconCode;
        else oldMsg += ' ' + iconCode;
        $('#newmessage-' + chatId).val(oldMsg);
        $('#newmessage-' + chatId).focus();
    }
};

$('.emoticons').allemoticons(emoSetting);

var hiddenDiv = $(document.createElement('div')),
    content = null;
hiddenDiv.addClass('chat-hiddendiv');
$("body").append(hiddenDiv);

function autoResizeTextArea(txtArea) {
    var hiddenDivChat = $(".chat-hiddendiv");
    var content = $(txtArea).val();
    content = content.replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/\n/g, '<br>');
    hiddenDivChat.html(content + '<br class="lbr">');

    var height = hiddenDivChat.height();
    var currentHeight = $(txtArea).height();

    var divChatMessage = $("#messages-" + $(txtArea).attr("id").replace("newmessage-", ""));
    var divCommand = $("#chatRoom-" + $(txtArea).attr("id").replace("newmessage-", "") + " .chat-command");

    var offset = (height - currentHeight);

    divChatMessage.css('height', divChatMessage.height() - offset);

    //divCommand.css('height', divCommand.height() + offset);

    $(txtArea).css('height', height);

    if (height == parseInt($(hiddenDivChat).css('max-height').replace("px"))) {
        $(txtArea).css('overflow', "auto");
    } else {
        $(txtArea).css('overflow', "hidden");
    }
}

function chatTextEnter(txtArea, evt) {
    var keycode = (evt.which) ? evt.which : evt.keyCode;
    if (evt.ctrlKey && keycode == 13) {
        $(txtArea).val($(txtArea).val() + "\n");
        autoResizeTextArea(txtArea);
        return false;
    }
}
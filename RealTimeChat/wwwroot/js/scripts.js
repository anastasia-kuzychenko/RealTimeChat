$("#loginform").on("blur", "input", () => {
    validateConectButton()
});

$("#loginform").on("focus", "button", () => {
    validateConectButton()
});

hideMessagePanel();

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/messages")
    .withAutomaticReconnect()
    .build();

connection.on('Send', (message) => {
    appendMessage(message.sender, message.text);
});

connection.on('OnNewUserConnected', (text) => {
    appendNotification(`${text} connected`, 'connected');
});

connection.on('OnDisconnected', (text) => {
    appendNotification(`${text} disconnected`, 'disconected');
});

connection.on('GetHistory', (messages) => {
    messages.forEach(appendHistoryMessage)
});

function validateConectButton() {
    if ($("#loginform").valid()) {
        $("#connectButton").removeAttr("disabled");
    } else {
        $("#connectButton").attr("disabled", "disabled");
    }
}

function showMessagePanel() {
    $("#message-sender").show();
    $("#message-box").show();
}

function hideMessagePanel() {
    $("#message-sender").hide();
    $("#message-box").hide();
}

function appendMessage(sender, text, isMy) {
    document.querySelector('#messages-content').insertAdjacentHTML("beforeend", `<div class = "message ${isMy ? "my" : ""}"><p>Sender: ${sender}</p><p  class="message-text">${text}</p></div>`);
}

function appendNotification(text, notificationType) {
    document.querySelector('#messages-content').insertAdjacentHTML("beforeend", `<div class = "notification ${notificationType ?? ""}"><p>Notification: ${text}</p></div>`);
}

function appendHistoryMessage(message) {
    appendMessage(message.sender, message.text);
}

async function connect() {
    if (connection.state === 'Disconnected') {
        try {
            await connection.start();
            let nickname = document.querySelector('#nickname').value;
            await connection.send('Connect', nickname);
        }
        catch (error) {
            console.log(error);
        }
        if (connection.state === 'Connected') {
            document.querySelector('#conState').textContent = 'Connected';
            document.querySelector('#conState').style.color = 'green';
            document.querySelector('#connectButton').textContent = 'Disconnect';
            showMessagePanel();
            $("#nickname").attr("disabled", "disabled");
        }
    } else if (connection.state === 'Connected') {
        await connection.stop();
        hideMessagePanel();
        $("#nickname").removeAttr("disabled");
        const item = document.querySelector('#messages-content');
        while (item.firstChild) {
            item.removeChild(item.firstChild)
        }
        document.querySelector('#conState').textContent = 'Disconnected';
        document.querySelector('#conState').style.color = 'red';
        document.querySelector('#connectButton').textContent = 'Connect';
    }
};

async function sendMessage() {
    if (connection.state === 'Connected') {
        let textArea = document.querySelector('#message');
        try {
            let text = textArea.value;
            await connection.send('SendToOthers', text);
            appendMessage('Me', text, 'my');
        }
        catch (error) {
            console.log(error);
        }
        document.querySelector('#message').value = '';
    }
}
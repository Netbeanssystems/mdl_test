let botScript = {

    registerMutationCheck: true,
    debugMode: false,

    toggleBotFrame: function () {

        const botFrame = document.getElementById("bot-container");
        const launchButton = document.getElementById("bot-launch-button");

        if (botFrame.classList.contains("bot-frame-hidden")) {
            botFrame.classList.remove("bot-frame-hidden");
            launchButton.style.display = "block";
        } else {
            botFrame.classList.add("bot-frame-hidden");
        }

        if (botScript.registerMutationCheck) {
            botScript.registerMutationCheck = false;
            var frameMutationObserver = new MutationObserver(function (mutationList) {
                if (botScript.debugMode) console.log(mutationList);
                for (let mutation of mutationList) {
                    if (mutation.type === "attributes" && mutation.target.isSameNode(botFrame)) {
                        if (botFrame.classList.contains("bot-frame-hidden")) {
                            launchButton.style.display = "block";
                        }
                        else if (botFrame.classList.contains("bot-ready")) {
                            launchButton.style.display = "none";
                        }
                    }
                }
            });

            frameMutationObserver.observe(botFrame, {
                'attributes': true
            });
        }
    },

    initialize: function () {
        //Attach send to input box Enter key.
        const inputBox = document.querySelector("#bot-input-box");
        inputBox.addEventListener("keyup", (event) => {
            if (event.key === "Enter") {
                botScript.sendMessage();
            }
        });

        //const parentFrame = window.frameElement;
        //parentFrame.classList.add("bot-ready");

        const closeButton = document.querySelector(".bot-title-close-button");
        closeButton.addEventListener("click", botScript.closeBot);
    },

    closeBot: function () {
        const closeButton = document.querySelector("#bot-container");
        closeButton.classList.add("bot-frame-hidden");
    },

    addUserMessage: function (userText) {
        const messageItem = document.querySelector(".message-template").cloneNode(true).firstElementChild;
        const messageList = document.querySelector(".bot-message-list");

        //add user class.
        messageItem.classList.add("user");

        const messageText = messageItem.querySelector(".message-text");
        messageText.innerHTML = userText;
        messageList.appendChild(messageItem);

        setTimeout(function () {
            return messageItem.classList.add("appeared");
        }, 0);

        botScript.scrollDown();
    },

    addBotMessage: function (channelResponse) {
        const messageItem = document.querySelector(".message-template").cloneNode(true).firstElementChild;
        const messageList = document.querySelector(".bot-message-list");

        //add bot class.
        messageItem.classList.add("bot");

        var botResponse = channelResponse.botResponse;

        //Show a temporary typing indicator.
        const indicatorItem = document.querySelector(".indicator-template").cloneNode(true).firstElementChild;
        const messageElement = messageItem.querySelector(".message-text");
        messageElement.appendChild(indicatorItem);
        messageList.appendChild(messageItem);

        if (botScript.debugMode) console.log(indicatorItem);

        setTimeout(function () {
            return messageItem.classList.add("appeared");
        }, 0);

        botScript.scrollDown();

        //After a delay remove the indicator and put the actual values.
        setTimeout(function () {

            indicatorItem.remove();
            messageElement.innerHTML = botResponse.text;

            //There are quick replies.
            if (botResponse.messages) {
                const textWrapper = messageItem.querySelector(".message-wrapper");
                const ul = document.createElement("ul");

                for (let item of botResponse.messages) {

                    //Text message.
                    if (item.type === "text") {

                        if (item.texts) {
                            const textArray = item.texts;
                            //Get random text value.
                            const randomText = textArray[Math.floor(Math.random() * textArray.length)];

                            messageElement.innerHTML = randomText;
                        }
                    }
                    //Quick reply message.
                    else if (item.type === "quick-replies") {
                        messageElement.innerHTML = item.title;
                        for (let reply of item.replies) {
                            const div = document.createElement("div");
                            div.className = "quick-reply";
                            div.textContent = reply;

                            //Click event on quick reply div.
                            div.addEventListener("click", () => {
                                botScript.processUserMessage(reply);
                                ul.remove();
                            });

                            const li = document.createElement("li");
                            li.appendChild(div);
                            ul.appendChild(li);
                        }
                    }
                    //Image message.
                    else if (item.type === "image") {
                        const imageElement = document.createElement("img");
                        imageElement.setAttribute("src", item.url);

                        messageElement.appendChild(imageElement);
                    }
                    else if (item.type === "basic-card") {
                        messageElement.innerHTML = item.title;

                        const imageElement = document.createElement("img");
                        imageElement.setAttribute("src", item.imageUrl);
                        messageElement.appendChild(imageElement);

                        const para = document.createElement("p");

                        let desc = item.description;
                        desc = desc.replace(/(?:\r\n|\r|\n)/g, '<br/>');
                        para.innerHTML = desc;
                        messageElement.appendChild(para);
                    }
                }

                textWrapper.appendChild(ul);
            }

            messageList.appendChild(messageItem);

            //Bind click event for anchor tag with bot_link class.
            messageItem.querySelectorAll(".bot-link").forEach(function (item) {
                item.addEventListener("click", function () {
                    botScript.processUserMessage(item.innerText);
                });
            });

            botScript.scrollDown();
        }, 500);
    },

    //Scroll to the bottom of the message-row grid
    scrollDown: function () {
        const messageList = document.querySelector(".bot-message-list");
        messageList.lastElementChild.scrollIntoView({ behavior: "smooth" });
    },

    processUserMessage: function (userTextMessage) {
        if (botScript.debugMode) console.log(`Processing user message: "${userTextMessage}"`);

        const userText = userTextMessage.trim();

        if (userText !== null && userText !== "") {

            botScript.addUserMessage(userText);

            const requestResponse = async () => {
                const botUserId = sessionStorage.getItem("bot_user_id");
                const response = await fetch(`Default.aspx&Query=${userText}`);
                const channelResponse = await response.json(); //extract JSON from the http response

                // do something with myJson
                if (botScript.debugMode) console.log(channelResponse);

                botScript.addBotMessage(channelResponse);
            }

            requestResponse();
        }
    },

    sendMessage: function () {

        const messageInput = document.querySelector("#bot-input-box");
        const userText = messageInput.value;

        //process the user message.
        botScript.processUserMessage(userText);

        //Reset to empty value.
        messageInput.value = "";
    }
};

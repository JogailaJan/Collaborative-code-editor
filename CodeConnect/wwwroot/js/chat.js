﻿"use strict";

//var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable the send button until connection is established.
//document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var p = document.createElement("p");
    document.getElementById("messagesList").appendChild(p);
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you
    // should be aware of possible script injection concerns.
    p.textContent = `${user} says ${message}`;
});

//connection.start().then(function () {
    //document.getElementById("sendButton").disabled = false;
//}).catch(function (err) {
    //return console.error(err.toString());
//});

//document.getElementById("sendButton").addEventListener("click", async function (event) {
    //var message = document.getElementById("messageInput").value;
    //connection.invoke("SendMessage", userName, message).catch(function (err) {
        //return console.error(err.toString());
    //});
    //event.preventDefault();
//});




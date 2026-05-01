window.mqttInterop = {
    connect: function (url, topic, dotnetRef, username, password) {

        const options = {};

        if (username && username.length > 0) {
            options.username = username;
            options.password = password;
        }

        const client = mqtt.connect(url, options);

        client.on('connect', function () {
            console.log("MQTT connected");
            client.subscribe(topic);
        });

        client.on('message', function (topic, message) {
            dotnetRef.invokeMethodAsync('OnMqttMessage', topic, message.toString());
        });

        client.on('error', function (err) {
            console.error("MQTT error", err);
        });

        window.mqttClient = client;
    }
};

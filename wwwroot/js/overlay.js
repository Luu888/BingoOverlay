const connection =
    new signalR.HubConnectionBuilder()
        .withUrl("/bingoHub")
        .build();


connection.on(
    "TileUpdated",
    function (id, completed) {
        const tile =
            document.querySelector(
                `[data-id='${id}']`
            );


        if (tile) {
            tile.classList.toggle(
                "done",
                completed
            );
        }
    });


connection.on(
    "TileTextUpdated",
    function (id, text) {

        const tile =
            document.querySelector(
                `[data-id='${id}']`
            );

        if (tile) {
            const textElement =
                tile.querySelector(".tile-text");

            if (textElement) {
                textElement.innerText = text;
            }
        }
    });


connection.on(
    "BoardReset",
    function () {
        console.log("Bingo reset");


        document
            .querySelectorAll(".tile")
            .forEach(tile => {
                tile.classList.remove("done");
            });
    });

connection.on(
    "AppearanceUpdated",
    function (app) {

        console.log("Nowy wygląd:", app);


        document.documentElement.style
            .setProperty(
                "--tile-start",
                app.tileColor
            );


        document.documentElement.style
            .setProperty(
                "--tile-end",
                app.tileColorEnd
            );


        document.documentElement.style
            .setProperty(
                "--done-start",
                app.completedColor
            );


        document.documentElement.style
            .setProperty(
                "--done-end",
                app.completedColorEnd
            );


        document.documentElement.style
            .setProperty(
                "--tile-text",
                app.textColor
            );


        document.documentElement.style
            .setProperty(
                "--radius",
                app.borderRadius + "px"
            );


        document.documentElement.style
            .setProperty(
                "--size",
                app.tileSize + "px"
            );

    });

connection.on(
    "OverlayVisibilityChanged",
    function (data) {

        console.log("Overlay visibility:", data);


        if (data.visible) {

            document.body.classList.remove(
                "overlay-hidden"
            );

        } else {

            document.body.classList.add(
                "overlay-hidden"
            );

        }

    });

connection.start()
    .then(() => {
        console.log("SignalR connected");
    })
    .catch(err => {
        console.error(err);
    });
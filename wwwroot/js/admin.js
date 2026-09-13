const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

document.querySelectorAll(".tile").forEach(button => {
    button.addEventListener("click", async () => {
        const id = button.dataset.id;
        const response = await fetch(`/Admin?handler=Toggle&id=${id}`, {
            method: "POST",
            headers: {
                "RequestVerificationToken": token
            }
        });
        if (!response.ok) {
            console.error("Toggle error", response.status);
        }
    });
});


document.querySelectorAll(".save-text").forEach(button => {
    button.addEventListener("click", async () => {
        const id = button.dataset.id;
        const input = document.querySelector(`.tile-text[data-id='${id}']`);
        const text = input.value;
        const response = await fetch(`/Admin?handler=UpdateText`, {
            method: "POST",
            headers: {
                "Content-Type":
                    "application/x-www-form-urlencoded",
                "RequestVerificationToken": token
            },
            body: `id=${id}&text=${encodeURIComponent(text)}`
        });

        if (!response.ok) {
            console.error("Update text error", response.status);
        }
    });
});


const connection = new signalR.HubConnectionBuilder()
    .withUrl("/bingoHub")
    .build();

connection.on("TileUpdated", function (id, completed) {
    const tile = document.querySelector(`.tile[data-id='${id}']`);
    if (tile) {
        tile.classList.toggle("done", completed);
    }
});

connection.on("TileTextUpdated", function (id, text) {
    const label = document.querySelector(`.tile[data-id='${id}'] .tile-label`);
    const input = document.querySelector(`.tile-text[data-id='${id}']`);

    if (label) {
        label.innerText = text;
    }

    if (input) {
        input.value = text;
    }

});

connection.start().then(() => {
    console.log("SignalR connected");
}).catch(err => {
    console.error(err);
});

document.querySelectorAll(".countdown-duration")
    .forEach(button => {
        button.addEventListener("click", async () => {
            const minutes = Number(button.dataset.minutes);

            const response = await fetch("?handler=SetCountdownDuration", {
                method: "POST",
                headers: {
                    "Content-Type": "application/x-www-form-urlencoded",
                    "RequestVerificationToken": token
                },
                body: `minutes=${encodeURIComponent(minutes)}`
            });

            if (!response.ok) {
                console.error(
                    "SetCountdownDuration error",
                    response.status
                );
            }
        });
    });


document.getElementById("startCountdown")
    ?.addEventListener("click", async () => {

        const response = await fetch("?handler=StartCountdown", {
            method: "POST",
            headers: {
                "RequestVerificationToken": token
            }
        });

        if (!response.ok) {
            console.error(
                "StartCountdown error",
                response.status
            );
        }
    });


document.getElementById("pauseCountdown")
    ?.addEventListener("click", async () => {

        const response = await fetch("?handler=PauseCountdown", {
            method: "POST",
            headers: {
                "RequestVerificationToken": token
            }
        });

        if (!response.ok) {
            console.error(
                "PauseCountdown error",
                response.status
            );
        }
    });


document.getElementById("resetCountdown")
    ?.addEventListener("click", async () => {

        const response = await fetch("?handler=ResetCountdown", {
            method: "POST",
            headers: {
                "RequestVerificationToken": token
            }
        });

        if (!response.ok) {
            console.error(
                "ResetCountdown error",
                response.status
            );
        }
    });


document.getElementById("saveCountdownAppearance")
    ?.addEventListener("click", async () => {

        const digitColor =
            document.getElementById("countdownDigitColor").value;

        const backgroundColor =
            document.getElementById("countdownBackgroundColor").value;

        const transparentBackground =
            document.getElementById("countdownTransparentBackground").checked;

        const body = new URLSearchParams({
            digitColor,
            backgroundColor,
            transparentBackground
        });

        const response = await fetch("?handler=SaveCountdownAppearance", {
            method: "POST",
            headers: {
                "Content-Type": "application/x-www-form-urlencoded",
                "RequestVerificationToken": token
            },
            body
        });

        if (!response.ok) {
            console.error(
                "SaveCountdownAppearance error",
                response.status
            );
        }
    });
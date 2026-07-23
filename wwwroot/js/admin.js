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
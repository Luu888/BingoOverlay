const connection =
    new signalR.HubConnectionBuilder()
        .withUrl("/bingoHub")
        .withAutomaticReconnect()
        .build();

let countdownState = {
    durationSeconds: 600,
    remainingSeconds: 600,
    isRunning: false,
    endsAt: null,
    digitColor: "#ffffff",
    backgroundColor: "#000000",
    transparentBackground: true
};

if (window.initialCountdown) {
    countdownState = {
        ...countdownState,
        ...window.initialCountdown
    };
}

function getRemainingSeconds() {
    if (!countdownState.isRunning || !countdownState.endsAt) {
        return Math.max(0, countdownState.remainingSeconds);
    }

    const endsAt = new Date(countdownState.endsAt).getTime();

    const remaining =
        Math.ceil((endsAt - Date.now()) / 1000);

    return Math.max(0, remaining);
}

function formatTime(totalSeconds) {
    const minutes = Math.floor(totalSeconds / 60);
    const seconds = totalSeconds % 60;

    return `${String(minutes).padStart(2, "0")}:${String(seconds).padStart(2, "0")}`;
}

function applyAppearance() {
    document.documentElement.style.setProperty(
        "--countdown-digit-color",
        countdownState.digitColor
    );

    if (countdownState.transparentBackground) {
        document.body.style.background = "transparent";
    } else {
        document.body.style.background =
            countdownState.backgroundColor;
    }
}

function renderCountdown() {
    const element =
        document.getElementById("countdownTime");

    if (!element) {
        return;
    }

    const remaining = getRemainingSeconds();

    if (remaining <= 0) {
        element.textContent = "STARTING";

        if (countdownState.isRunning) {
            countdownState.isRunning = false;
            countdownState.remainingSeconds = 0;
            countdownState.endsAt = null;
        }

        return;
    }

    element.textContent = formatTime(remaining);
}

connection.on("CountdownUpdated", function (data) {
    countdownState = {
        ...countdownState,
        ...data
    };

    applyAppearance();
    renderCountdown();
});

connection.start()
    .then(() => {
        console.log("Countdown SignalR connected");
    })
    .catch(error => {
        console.error("Countdown SignalR error:", error);
    });

applyAppearance();
renderCountdown();

setInterval(renderCountdown, 250);
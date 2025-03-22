document.addEventListener("DOMContentLoaded", function () {
    var mistakes = 0;
    var matchedCount = 0;

    var enArray = shuffle(cards.slice());
    var uaArray = shuffle(cards.slice());

    function shuffle(array) {
        for (let i = array.length - 1; i > 0; i--) {
            const j = Math.floor(Math.random() * (i + 1));
            [array[i], array[j]] = [array[j], array[i]];
        }
        return array;
    }

    function renderLists() {
        var enListEl = document.getElementById("enList");
        var uaListEl = document.getElementById("uaList");
        enListEl.innerHTML = "";
        uaListEl.innerHTML = "";

        enArray.forEach(function (item) {
            var btn = document.createElement("button");
            btn.className = "btn btn-outline-primary mb-2 word-button";
            btn.setAttribute("data-id", item.Id);
            btn.innerText = item.WordEn;
            btn.onclick = function () { selectWord(btn, 'en'); };
            enListEl.appendChild(btn);
        });

        uaArray.forEach(function (item) {
            var btn = document.createElement("button");
            btn.className = "btn btn-outline-primary mb-2 word-button";
            btn.setAttribute("data-id", item.Id);
            btn.innerText = item.WordUa;
            btn.onclick = function () { selectWord(btn, 'ua'); };
            uaListEl.appendChild(btn);
        });

        updateScoreDisplay();
    }

    function updateScoreDisplay() {
        var totalAttempts = matchedCount + mistakes;
        var score = totalAttempts > 0 ? Math.round((matchedCount / totalAttempts) * 100) : 0;
        document.getElementById("scoreDisplay").innerText = score;
    }

    var selectedEn = null;
    var selectedUa = null;

    function selectWord(button, lang) {
        if (button.classList.contains("matched")) return;

        if (lang === 'en') {
            if (selectedEn) selectedEn.classList.remove("btn-warning");
            selectedEn = button;
            button.classList.add("btn-warning");
        } else if (lang === 'ua') {
            if (selectedUa) selectedUa.classList.remove("btn-warning");
            selectedUa = button;
            button.classList.add("btn-warning");
        }

        if (selectedEn && selectedUa) {
            checkMatch();
        }
    }

    function checkMatch() {
        var enId = selectedEn.getAttribute("data-id");
        var uaId = selectedUa.getAttribute("data-id");

        if (enId === uaId) {
            selectedEn.classList.remove("btn-warning");
            selectedUa.classList.remove("btn-warning");
            selectedEn.classList.add("btn-success", "matched");
            selectedUa.classList.add("btn-success", "matched");
            selectedEn.disabled = true;
            selectedUa.disabled = true;

            matchedCount++;
        } else {
            mistakes++;
            selectedEn.classList.remove("btn-warning");
            selectedUa.classList.remove("btn-warning");

            selectedEn.classList.add("btn-danger");
            selectedUa.classList.add("btn-danger");
        }

        selectedEn = null;
        selectedUa = null;
        updateScoreDisplay();

        if (matchedCount === totalPairs) {
            document.getElementById("finishTest").disabled = false;
        }
    }

    document.getElementById("finishTest").onclick = function () {
        var finalScore = Math.round((matchedCount / (matchedCount + mistakes)) * 100);
        document.getElementById("scoreInput").value = finalScore;
        document.getElementById("scoreForm").submit();
    };

    renderLists();
});

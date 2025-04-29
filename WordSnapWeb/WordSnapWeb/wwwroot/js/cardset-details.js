document.addEventListener('DOMContentLoaded', function () {
    const flipCards = document.querySelectorAll('.flip-card');
    const prevButton = document.getElementById('prev-card');
    const nextButton = document.getElementById('next-card');
    const currentCardElement = document.getElementById('current-card');

    let currentCardIndex = 0;

    if (flipCards.length > 0) {
        flipCards[0].classList.add('active');
        updateCardPosition();
    }

    flipCards.forEach(card => {
        const front = card.querySelector('.flip-card-front');
        const back = card.querySelector('.flip-card-back');

        front.addEventListener('click', () => {
            card.classList.add('flipped');
        });

        back.addEventListener('click', () => {
            card.classList.remove('flipped');
        });
    });

    if (prevButton) {
        prevButton.addEventListener('click', () => {
            flipCards[currentCardIndex].classList.remove('active');
            flipCards[currentCardIndex].classList.remove('flipped');

            currentCardIndex = (currentCardIndex - 1 + flipCards.length) % flipCards.length;

            flipCards[currentCardIndex].classList.add('active');
            updateCardPosition();
            currentCardElement.textContent = currentCardIndex + 1;
        });
    }

    if (nextButton) {
        nextButton.addEventListener('click', () => {
            flipCards[currentCardIndex].classList.remove('active');
            flipCards[currentCardIndex].classList.remove('flipped');

            currentCardIndex = (currentCardIndex + 1) % flipCards.length;

            flipCards[currentCardIndex].classList.add('active');
            updateCardPosition();
            currentCardElement.textContent = currentCardIndex + 1;
        });
    }

    function updateCardPosition() {
        flipCards.forEach((card, index) => {
            if (index === currentCardIndex) {
                card.style.transform = 'translateX(0)';
                card.style.opacity = '1';
                card.style.zIndex = '5';
            } else if (index < currentCardIndex) {
                card.style.transform = 'translateX(-100%)';
                card.style.opacity = '0';
                card.style.zIndex = '1';
            } else {
                card.style.transform = 'translateX(100%)';
                card.style.opacity = '0';
                card.style.zIndex = '1';
            }
        });
    }
});
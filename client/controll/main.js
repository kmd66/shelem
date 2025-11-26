import { Deck } from './deck';
//import { gsap } from "gsap";
import { deckControll } from './deckControll';
import { Propertys as p } from './public';

document.addEventListener('DOMContentLoaded', () => {
    p.init();
    d1()
});
async function d1() {

    const suits = [0, 1, 2, 3]; // ♠, ♥, ♣, ♦
    const ranks = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13];

    const allCards = [];

    for (let suit of suits) {
        for (let rank of ranks) {
            allCards.push({ suit, rank });
        }
    }
    const shuffled = [...allCards].sort(() => Math.random() - 0.5);
    const t = shuffled.slice(0, 12);

    const gList = {
         //u1: [{ count: 4, suit: 0, rank: 5 }, { count: 1, suit: 3, rank: 2 }, { count: 3, suit: 1, rank: 5}],
       u1: [{ count: 4, suit: -1, rank: -1 }, { count: 3, suit: -1, rank: -1 }, { count: 1, suit: -1, rank: -1 }],
        u2: [{ count: 4, suit: -1, rank: -1 }, { count: 3, suit: -1, rank: -1 }, { count: 1, suit: -1, rank: -1 }],
    }

    //await deckControll.start()
    deckControll.startFan(t)
    deckControll.startGround(gList)

}


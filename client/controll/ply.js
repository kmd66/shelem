import { gsap } from "gsap";
import { Propertys as p } from './public';

class Ply {
    constructor() {
    }

    static instance;
    static Instance() {
        if (!Ply.instance) {
            Ply.instance = new Ply();
        }
        return Ply.instance;
    }
}

if (!window.__ply__) {
    window.__ply__ = Ply.Instance();
}
export const ply = window.__ply__;

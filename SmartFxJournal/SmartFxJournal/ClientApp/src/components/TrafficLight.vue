<script lang="ts">
    export default {
        props: {
            currenValue: {type: String, required: true},
            lightValues: {type: Array<string>, required: true}
        },
        methods: {
            getFormat(val : string) {
                let cellClass = 'trafficLight';
                if (val == this.lightValues[0]) {
                    cellClass += ' redLight';
                } else if (val == this.lightValues[1]) {
                    cellClass += ' orangeLight';
                } else if (val == this.lightValues[2]) {
                    cellClass += ' greenLight';
                } else {
                    cellClass += ' greyLight';
                }

                cellClass += (val == this.currenValue) ? ' on' : ' off';
                return cellClass;
            }
        } 
    }
</script>

<template>
    <div class="trafficLightBox">
        <div id="red" :class="getFormat(lightValues[0])"/>
        <div id="orange" :class="getFormat(lightValues[1])"/>
        <div id="green" :class="getFormat(lightValues[2])"/>
    </div>
</template>

<style scoped>
    .trafficLightBox {
        display: flex;
        flex-direction: row;
        background: #222;
        background-image: linear-gradient(transparent 2%, #111 2%, transparent 3%, #111 30%);
        width: fit-content;
        height: 18px;
        border-radius: 5px;
        position: relative;
        border: solid 5px #333;
    }
    .trafficLight {
        height: 15px;
        width: 15px;
        border-radius: 50%;
        margin-right: 1px;
    }
    .greyLight {
        background-color: gainsboro;
        border: dotted 1px black;
    }
    .redLight {
        background: red;
        background-image: radial-gradient(orangered, transparent);
        background-size: 3px 3px; 
        border: dotted 1px red;
        box-shadow: 
            0 0 2px #111 inset,
            0 0 1px red;
    }
    .greenLight {
        background: green;
        background-image: radial-gradient(lime, transparent);
        background-size: 3px 3px;
        border: dotted 1px darkgreen;
        box-shadow: 
            0 0 2px #111 inset,
            0 0 1px lime;
    }
    .orangeLight {
        background: yellow;
        background-image: radial-gradient(orange, transparent);
        background-size: 3px 3px;
        border: dotted 1px goldenrod;
        box-shadow: 
            0 0 2px #111 inset,
            0 0 1px yellow;
    }
    .on {
        opacity: 100%;
    }
    .off {
        opacity: 35%;
    }
</style>
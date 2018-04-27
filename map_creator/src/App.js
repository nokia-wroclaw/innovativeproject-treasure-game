import React, { Component } from 'react';
import Map from "./Map";

import './App.css';

class App extends Component {
    render() {
        return (
            <div className="App">
                <header className="App-header">
                    <h1 className="App-title">Steal the treasure game - Map editor</h1>
                </header>
                <p className="App-intro">
                </p>
                <Map />
            </div>
        );
    }
}
export default App;

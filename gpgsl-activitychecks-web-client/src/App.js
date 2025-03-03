import React from 'react';
import { useState, useEffect } from 'react';
import { collection, getDocs } from "firebase/firestore";
import { db } from './firebase';
import './App.css';

function App(){
  const [teams, setTeams] = useState([]);
  const [drivers, setDrivers] = useState([]);
  const [warnings, setWarnings] = useState([]);
  const [totals, setTotals] = useState([]);
  const [lastUpdatedTime, setLastUpdated] = useState("");
  const [lastUpdatedPage, setLastUpdatedPage] = useState("");

  const notPostedColor = "#F77F7F";
  const postedColor = "#A8E6A1";


  useEffect(() => {
    const fetchTeams = async () => {
      try {
        const teamsRef = collection(db, "teams");
        const teamDocs = await getDocs(teamsRef);
        const fetchedTeams = [];
        teamDocs.forEach(doc => {
          fetchedTeams.push({
            id: doc.id,
            name: doc.data().name,
            username: doc.data().username
          });
        });
        setTeams(fetchedTeams.sort((a, b) => a.id - b.id));
      } catch (error) {
        console.error("Error fetching teams: ", error);
      }
    };

    const fetchDrivers = async () => {
      try {
        const driversRef = collection(db, "drivers");
        const driverDocs = await getDocs(driversRef);
        const fetchedDrivers = [];
        driverDocs.forEach(doc => {
          fetchedDrivers.push({
            id: doc.id,
            name: doc.data().name,
            username: doc.data().username,
            team: doc.data().team,
          });
        });
        setDrivers(fetchedDrivers.sort((a, b) => a.id - b.id));
      } catch (error) {
        console.error("Error fetching drivers: ", error);
      }
    };
    
    const fetchWarnings = async () => {
      var docid;
      try {
        const warningsRef = collection(db, "warnings");
        const warningDocs = await getDocs(warningsRef);
        const fetchedWarnings = [];
        const fetchedTotals = [];

        warningDocs.forEach(doc => {
          const data = doc.data();
          docid = console.log(doc.id);
          
          if (doc.id === "notPosted") {
            //console.log(data.Data);
            const notPosted = JSON.parse(data.Data);
            // the output of my doc is a stringified JSON
            // here we try to find matches to the usernames
          
            notPosted.forEach(doc => {
              fetchedWarnings.push({
                Username: doc.Username
              });
            });
            setWarnings(fetchedWarnings);
            //console.log("Warnings: "+ fetchedWarnings);
          }

          if (doc.id === "total") {
            const total = JSON.parse(data.Data);
            // the output of my doc is a stringified JSON
            // here we try to find matches to the usernames
          
            console.log("Totals: "+ fetchedTotals);
            total.forEach(doc => {
              fetchedTotals.push({
                Username: doc.Username,
                Warnings: doc.Warnings
              });
              //console.log(doc);
            });
            setTotals(fetchedTotals);
            //console.log("Totals: "+ fetchedTotals);
          }

          if (doc.id === "lastUpdated") {
            //console.log(data.Data);
            setLastUpdated(JSON.parse(data.Data).LastUpdatedTime);
            setLastUpdatedPage(JSON.parse(data.Data).LastUpdatedPage);
          }
        });
        
      } catch (error) {
        console.log("Error on id: " + docid);
        console.error("Error fetching warnings: ", error);
      }
    };

    fetchTeams();
    fetchDrivers();
    fetchWarnings();
  }, []);

  return (
    <div>
      {/* Display Last Updated Time */}
      
      {/* Header */}
      <div style={{ 
        fontSize: "1.2em", 
        color: "red", 
        backgroundColor: '#282c34',
        color: 'white',
        padding: '10px 20px',
        width: '400px',
        marginTop: '0',
        textAlign: 'center'
      }}>
        Last Updated: {lastUpdatedTime} {/* - {lastUpdatedPage} */}<br />
        <p className="description">This app uses information up to the timestamp. It may not update in real-time.</p>
      </div>
      {/* Table (body) */}
      <div className="lineup-editor">
        <table>
          <thead>
            <tr>
              <th>User</th>
              <th className="has-posted">Has Posted</th>
              <th className="prev-warnings">Prev. warnings</th>
            </tr>
          </thead>
          <tbody>
            {teams.map((team) => (
              <React.Fragment key={team.id}>
                {/* Team Row */}
                <tr className="team-row">
                  <td>
                    {/* Team name and username */}
                    <div>
                      {team.id / 100}. {team.name} ({team.username})
                    </div>
                    {(team.short1 || team.short2) && (
                      <div style={{ fontSize: "0.6em", color: "gray" }}>
                        {team.short1 && <span>{team.short1}</span>}
                        {team.short1 && team.short2 && <span> · </span>}
                        {team.short2 && <span>{team.short2}</span>}
                      </div>
                    )}
                  </td>

                  {/* Not Posted Column */}
                  <td
                    style={{
                      backgroundColor: warnings.some(
                        (warning) => warning.Username === team.username
                      )
                        ? notPostedColor
                        : postedColor,
                      color: 'black',
                      textAlign: 'center'
                    }}
                  >
                    {warnings.some((warning) => warning.Username === team.username) ? "No" : ""}
                  </td>

                  {/* Total Column */}
                  <td className="prev-warnings">
                    {
                      totals.find(
                        (total) => total.Username === team.username
                      )?.Warnings ?? ""
                    }
                  </td>
                </tr>

                {/* Driver Rows */}
                {drivers
                  .filter((driver) => Math.floor(driver.id / 100) === Math.floor(team.id / 100))
                  .map((driver) => (
                    <tr key={driver.id} className="driver-row">
                      <td style={{ paddingLeft: "20px" }}>
                        #{driver.id % 100}: {driver.name} ({driver.username})
                      </td>

                      {/* Not Posted for Driver */}
                      <td
                        style={{
                          backgroundColor: warnings.some(
                            (warning) => warning.Username === driver.username
                          )
                            ? notPostedColor
                            : postedColor,
                          color: 'black',
                          textAlign: 'center'
                        }}
                      >
                        {warnings.some((warning) => warning.Username === driver.username) ? "No" : ""}
                      </td>

                      {/* Total Column for Driver */}
                      <td className="prev-warnings">
                        {
                          totals.find(
                            (total) => total.Username === driver.username
                          )?.Warnings ?? ""
                        }
                      </td>
                    </tr>
                  ))}
              </React.Fragment>
            ))}
          </tbody>
        </table>
        <div className="credits">
          App version 0.1.0.<br />
          Contact: <a href="mailto:milos.ancevski@student.um.si">milos.ancevski@student.um.si</a><br />
          GitHub: <a href="https://github.com/MilosMiki/GPGorg_activity_check" target="_blank" rel="noopener noreferrer">https://github.com/MilosMiki/GPGorg_activity_check</a>
        </div>
      </div>
    </div>
  );
};

export default App;

import "../App.css"

  
export const Star = ({name, time}) => {
  return(
    <div className="Star">
      <svg
        className="Star"
        xmlns="http://www.w3.org/2000/svg"
        viewBox="0 0 200 200"
      >
        <path
          d="M157.096 184.718l-60.01-31.565-60.025 31.542 11.476-66.828-48.546-47.34 67.103-9.737L97.115-.008l29.997 60.811 67.099 9.764-48.564 47.319z"
          fill="#fc0"
        />
        <path d="M127.315 60.416l-30.72 41.819 97.411-31.899z" fill="#ffe680" />
        <path
          d="M97.095 101.346v51.942l-60.63 31.117zM97.095 101.346l59.613 81.476-11.189-65.984z"
          fill="#fd5"
        />
        <path d="M.385 70.406L97.1 101.348 67.218 60.506z" fill="#ffe680" />
        <path d="M97.095 101.346V.126l29.83 60.357z" fill="#fd5" />
        <path d="M37.085 183.566l11.261-66.541 48.757-15.679z" fill="#ffd42a" />
        <text fill="#000000" x="48%" y="30%" alignmentBaseline="middle" textAnchor="middle" fontSize="32" fontFamily="Arial Black">1st</text>
        <text fill="#000000" x="50%" y="50%" alignmentBaseline="middle" textAnchor="middle" fontSize="25" fontFamily="Arial Black">{name}</text>
        <text fill="#000000" x="50%" y="60%" alignmentBaseline="middle" textAnchor="middle" fontSize="22" fontFamily="Arial Black">{time}</text>
      </svg>
    </div>
  )
}
